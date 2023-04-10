using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class StateTreeFactory
{
    public static StateTreeMutable Create(MapEntity map, SessionConf conf, int exposingMaxDepth){
        var tree = new StateTreeMutable(map, conf);
        ExposeTree(tree, map, conf, 10);

        // Debug.Log("Player");
        // foreach (var child in tree.CurrentStateNode.Children[0].Children[0].Children){
        //     child.PlayerVisitedPositions.Print();
        // }
        // Debug.Log("Enemy");
        // foreach (var child in tree.CurrentStateNode.Children[0].Children[0].Children){
        //     child.AIVisitedPositions.Print();
        // }

        // MutableStateTreeNode node = tree.CurrentStateNode;
        // for (int i=0; i<5; i++){
        //     foreach (var child in node.Children)
        //         child.PlayerVisitedPositions.Print($"{i} player");
        //     foreach (var child in node.Children)
        //         child.AIVisitedPositions.Print($"{i} oponent");
            
        //     node = node.Children[2];
        // }

        return tree;
    }

    private static void ExposeTree(StateTreeMutable tree, MapEntity map, SessionConf conf, int exposingMaxDepth){
        _ExposeNodeRecursive(tree.CurrentStateNode, map, conf, 0, exposingMaxDepth);
    }


    private static void _ExposeNodeRecursive(MutableStateTreeNode exposeNode, MapEntity map, SessionConf conf, 
        int exposedNodeDepth, int exposeMaxDepth)
    {
        // Debug.Log(exposedNodeDepth);
        if (exposedNodeDepth >= exposeMaxDepth
            || _IsSnakesOverlaping(exposeNode))
        {
            if (_IsSnakesOverlaping(exposeNode)){
                // exposeNode.PlayerVisitedPositions.Print("player");
                // exposeNode.AIVisitedPositions.Print("oponent");
            }
            return;
        }

        bool wasExposedNodePlayerTurn = conf.IsPlayerStarting ? exposedNodeDepth%2==0 : exposedNodeDepth%2==1;

        foreach (var direction in GetAvaiableDirection(exposeNode, map, wasExposedNodePlayerTurn))
        {
            var newNode = CreateChildMutableStateTreeNode(map, exposeNode, direction, wasParentPlayerTurn:wasExposedNodePlayerTurn);
            _ExposeNodeRecursive(newNode, map, conf, exposedNodeDepth+1, exposeMaxDepth);
            exposeNode.Children.Add(newNode);
        }
        
        foreach (var child in exposeNode.Children){
            // child.PlayerVisitedPositions.Print("player ");
            // child.AIVisitedPositions.Print("oponent ");
        }
    }



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

    private static MutableStateTreeNode CreateChildMutableStateTreeNode(MapEntity map, MutableStateTreeNode parent,
        Vector2 direction, bool wasParentPlayerTurn)
    {
        Vector2 newPosition = direction + (wasParentPlayerTurn ? parent.PlayerVisitedPositions.Last() : parent.AIVisitedPositions.Last());

        var playerVisitedPosition = wasParentPlayerTurn ? parent.PlayerVisitedPositions.DuplicateAndAdd(newPosition) : parent.PlayerVisitedPositions;
        var aiVisitedPositions = (!wasParentPlayerTurn) ? parent.AIVisitedPositions.DuplicateAndAdd(newPosition) : parent.AIVisitedPositions;
        var foodPositions = parent.FoodPositions.Contains(newPosition) 
            ? parent.FoodPositions.DuplicateAndRemove(newPosition) : parent.FoodPositions;
        var treeNode = new MutableStateTreeNode(parent, playerVisitedPosition, aiVisitedPositions, foodPositions);
        return treeNode;
    }


    
    // public static void ExposeCurrentStateRecursive(MapEntity map, MutableStateTreeNode currentNode, bool isPlayerTurn, int exposingDepth){
    //     // Debug.Log("expose current state / depth = 0");
    //     ExposeState(map, currentNode, -1, exposingDepth, isPlayerTurn);
    // }

    // private static void ExposeState(MapEntity map, MutableStateTreeNode exposeTarget, int nodeDepth, int maxDepth, bool isPlayerTurn){
    //     // Debug.Log($"expose state / depth = {nodeDepth}");
    //     Debug.Log("iteration");
    //     if (nodeDepth >= maxDepth 
    //         || IsSnakesOverlaping(exposeTarget)){
    //         exposeTarget.HeuristicEvaluation = CalculateLeafEvaluation(exposeTarget);
    //         return;
    //     }
        
    //     //is already exposed
    //     if (exposeTarget.Children.Count > 0){
    //         foreach (var stateNode in exposeTarget.Children){
    //             ExposeState(map, stateNode, nodeDepth+1, maxDepth, isPlayerTurn);
    //         return;
    //         }
    //     }
    //     //expose target node
    //     ProcessStateExposing(map, exposeTarget, !isPlayerTurn, nodeDepth, maxDepth);
    //     // Debug.Log($"iteration {exposeTarget.PlayerVisitedPositions.Last()} {exposeTarget.AIVisitedPositions.Last()}");
    // }

    // private static void ProcessStateExposing(MapEntity map, MutableStateTreeNode exposeTarget, bool isPlayerTurn, int nodeDepth, int exposingDepth){
    //     // Debug.Log($"expose node / depth = {nodeDepth}");
    // }
    
     


    // private static int CalculateLeafEvaluation(MutableStateTreeNode stateNode){
    //     int scoreComparison = stateNode.PlayerScore - stateNode.AIScore; //player always maximizer, ai always minimizer
    //     if (scoreComparison == 0) 
    //         return 0;
    //     else if (scoreComparison > 0) 
    //         return 1;
    //     else 
    //         return -1;
    // }

    // private static void EvalueateTreeRecursive(StateTreeMutable tree, bool IsPlayerStarting){
    //     MutableStateTreeNode lastEvaluatedNode;
    //     int nodeLayer;
    //     {
    //         var leaf = GetAnyLeaf(tree);
    //         lastEvaluatedNode = leaf.node;
    //         nodeLayer = leaf.layer;
    //     }

    //     do{
    //         lastEvaluatedNode = lastEvaluatedNode.Parent;
    //         EvaluateStateTreeNodeRecursive(lastEvaluatedNode, IsPlayerStarting ? nodeLayer%2==0 : nodeLayer%2==1);
    //         lastEvaluatedNode.PrintPlayerPath();
    //     }
    //     while (nodeLayer >= 0);
    // }

    // private static void EvaluateStateTreeNodeRecursive(MutableStateTreeNode node, bool IsPlayerTurn){
    //     foreach (var child in node.Children){
    //         if (child.HeuristicEvaluation != null)
    //             continue;
    //         EvaluateStateTreeNodeRecursive(child, !IsPlayerTurn);
    //     }
    // }

    

    // private static (MutableStateTreeNode node, int layer) GetAnyLeaf(StateTreeMutable tree){
    //     MutableStateTreeNode current = tree.CurrentStateNode;
    //     int currentLayer = 0;
    //     while (current.Children.Count > 0){
    //         currentLayer++;
    //         current = current.Children[0];
    //     }
    //     return (current, currentLayer);
    // }
}
