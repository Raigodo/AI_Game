using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class StateTreeFactory
{

    public static StateTreeMutable Create(MapEntity map, SessionConf conf, int exposingMaxDepth){
        var tree = new StateTreeMutable(map, conf);
        ExposeTree(tree, map, conf, exposingMaxDepth);
        _EvaluateTree(tree, map, conf);
        // Debug.Log(tree.CurrentStateNode.HeuristicEvaluation);
        return tree;
    }

    private static void ExposeTree(StateTreeMutable tree, MapEntity map, SessionConf conf, int exposingMaxDepth){
        _ExposeNodeRecursive(tree.CurrentStateNode, map, conf, 0, exposingMaxDepth);
    }


    private static void _ExposeNodeRecursive(MutableStateTreeNode exposeNode, MapEntity map, SessionConf conf, 
        int exposedNodeDepth, int exposeMaxDepth)
    {
        // Debug.Log(exposedNodeDepth);
        
        // exposeNode.PlayerVisitedPositions.Print("player ");
        // exposeNode.AIVisitedPositions.Print("enemy ");
        if (exposedNodeDepth >= exposeMaxDepth
            || _IsSnakesOverlaping(exposeNode)){
            return;
        }

        bool wasExposedNodePlayerTurn = conf.IsPlayerStarting ? exposedNodeDepth%2==0 : exposedNodeDepth%2==1;

        var avaiableDirections = GetAvaiableDirection(exposeNode, map, wasExposedNodePlayerTurn);
        if (avaiableDirections.Count() == 0){
            var newNode = _CreateChildMutableStateTreeNode(map, exposeNode, Vector2.zero, wasParentPlayerTurn:wasExposedNodePlayerTurn);
            exposeNode.Children.Add(newNode);
        }
        else{
            foreach (var direction in avaiableDirections)
            {
                var newNode = _CreateChildMutableStateTreeNode(map, exposeNode, direction, wasParentPlayerTurn:wasExposedNodePlayerTurn);
                exposeNode.Children.Add(newNode);
            }
        }
        

        foreach (var child in exposeNode.Children){
            _ExposeNodeRecursive(child, map, conf, exposedNodeDepth+1, exposeMaxDepth);
        }
    }

    private static int nodesGenerated = 0;


    private static bool _IsSnakesOverlaping(MutableStateTreeNode currentNode){
        if (currentNode.PlayerVisitedPositions.Contains(currentNode.AIVisitedPositions.Last())
            || currentNode.AIVisitedPositions.Contains(currentNode.PlayerVisitedPositions.Last()))
            return true;
        return false;
    }

    private static IEnumerable<Vector2> GetAvaiableDirection(MutableStateTreeNode targetNode, MapEntity map, bool isPlayerTurn){
        List<Vector2> allowedDirections = new List<Vector2>();
        Vector2[] visitedPositions = isPlayerTurn ? targetNode.PlayerVisitedPositions : targetNode.AIVisitedPositions;
        Vector2 currentPosition = visitedPositions.Last();

        bool isCellNotVisitedInDirection = !visitedPositions.Contains(currentPosition + Vector2.right);
        bool isFacingMapBorder = currentPosition.x == (map.Dimensions-1);
        if (isCellNotVisitedInDirection && !isFacingMapBorder)
            allowedDirections.Add(Vector2.right);

        isCellNotVisitedInDirection = !visitedPositions.Contains(currentPosition + Vector2.left);
        isFacingMapBorder = currentPosition.x == 0;
        if (isCellNotVisitedInDirection && !isFacingMapBorder)
            allowedDirections.Add(Vector2.left);
        
        isCellNotVisitedInDirection = !visitedPositions.Contains(currentPosition + Vector2.up);
        isFacingMapBorder = currentPosition.y == (map.Dimensions-1);
        if (isCellNotVisitedInDirection && !isFacingMapBorder)
            allowedDirections.Add(Vector2.up);

        isCellNotVisitedInDirection = !visitedPositions.Contains(currentPosition + Vector2.down);
        isFacingMapBorder = currentPosition.y == 0;
        if (isCellNotVisitedInDirection && !isFacingMapBorder)
            allowedDirections.Add(Vector2.down);
        
        return allowedDirections;
        // {
        //     var newStateNode = CreateChildMutableStateTreeNode(map, exposeTarget, turnOwnerCurrentPosition, Vector2.down, isPlayerTurn);
        //     exposeTarget.Children.Add(newStateNode);
        //     ExposeState(map, newStateNode, nodeDepth+1, exposingDepth, !isPlayerTurn);
        // }
    }

    private static MutableStateTreeNode _CreateChildMutableStateTreeNode(MapEntity map, MutableStateTreeNode parent,
        Vector2 direction, bool wasParentPlayerTurn)
    {
        Vector2 newPosition = direction + (wasParentPlayerTurn ? parent.PlayerVisitedPositions.Last() : parent.AIVisitedPositions.Last());

        var playerVisitedPosition = wasParentPlayerTurn ? parent.PlayerVisitedPositions.DuplicateAndAdd(newPosition) : parent.PlayerVisitedPositions;
        var aiVisitedPositions = (!wasParentPlayerTurn) ? parent.AIVisitedPositions.DuplicateAndAdd(newPosition) : parent.AIVisitedPositions;
        bool foodFoundAndEaten = parent.FoodPositions.Contains(newPosition);
        var foodPositions = foodFoundAndEaten ? parent.FoodPositions.DuplicateAndRemove(newPosition) : parent.FoodPositions;
        var treeNode = new MutableStateTreeNode(parent, playerVisitedPosition, aiVisitedPositions, foodPositions);
        treeNode.PlayerScore = parent.PlayerScore + ((foodFoundAndEaten && wasParentPlayerTurn) ? 1 : 0);
        treeNode.AIScore = parent.AIScore + ((foodFoundAndEaten && !wasParentPlayerTurn) ? 1 : 0);
        return treeNode;
    }

    private static void _EvaluateTree(StateTreeMutable tree, MapEntity map, SessionConf conf){
        (MutableStateTreeNode node, int layer) nodeData = GetAnyLeaf(tree);
        do{
            EvalueateStateNodeRecursive(nodeData.node, conf.IsPlayerStarting ? nodeData.layer%2==0 : nodeData.layer%2==1);
            nodeData.node = nodeData.node.Parent;
            nodeData.layer--;
        }
        while (nodeData.layer >= 0);
    }

    private static (MutableStateTreeNode, int) GetAnyLeaf(StateTreeMutable tree){
        MutableStateTreeNode node = tree.CurrentStateNode;
        int i=0;
        while (node.Children.Count() != 0){
            i++;
            node = node.Children[0];
        }
        return (node, i);
    }

    private static void _EvaluateLeafStateNode(MutableStateTreeNode node){
        int scoreDifference = node.PlayerScore-node.AIScore;
        node.HeuristicEvaluation = scoreDifference == 0 ? 0 : (int) Mathf.Sign(scoreDifference);
    }

    private static void EvalueateStateNodeRecursive(MutableStateTreeNode node, bool isPlayerTurn){
        int scoreDifference = node.PlayerScore-node.AIScore;

        if (node.Children.Count() <= 0){
            _EvaluateLeafStateNode(node);
            return;
        }
        else{
            foreach (var child in node.Children)
                EvalueateStateNodeRecursive(child, !isPlayerTurn);
        }

        node.HeuristicEvaluation = isPlayerTurn ? 
            node.Children.Max(child => child.HeuristicEvaluation) : 
            node.Children.Min(child => child.HeuristicEvaluation);
    }
}
