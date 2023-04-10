using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateTreeFactory
{
    public static StateTreeMutable Create(MapEntity map, SessionConf conf, int exposingDepth = 3){
        var tree = new StateTreeMutable(map, conf);
        ForceExposeNode(map, tree.CurrentStateNode, conf.IsPlayerStarting, exposingDepth:exposingDepth);
        tree.CurrentStateNode.Children[1].PrintPaths();
        // Debug.Log(tree.CurrentStateNode.Children[0].PlayerVisitedPositions.Length);
        return tree;
    }


    
    public static void ExposeCurrentState(MapEntity map, MutableStateTreeNode currentNode, bool isPlayerTurn, int exposingDepth = 3){
        // Debug.Log("expose current state / depth = 0");
        ExposeState(map, currentNode, -1, exposingDepth, isPlayerTurn);
    }

    private static void ExposeState(MapEntity map, MutableStateTreeNode exposeTarget, int nodeDepth, int maxDepth, bool isPlayerTurn){
        // Debug.Log($"expose state / depth = {nodeDepth}");
        if (nodeDepth >= maxDepth){
            return;
        }
        
        //is already exposed
        if (exposeTarget.Children.Count > 0)
            foreach (var stateNode in exposeTarget.Children){
                ExposeState(map, stateNode, nodeDepth+1, maxDepth, isPlayerTurn);
            return;
        }
        //expose target node
        ForceExposeNode(map, exposeTarget, !isPlayerTurn, nodeDepth);
    }

    private static void ForceExposeNode(MapEntity map, MutableStateTreeNode exposeTarget, bool isPlayerTurn, int nodeDepth = 0, int exposingDepth = 3){
        isPlayerTurn = true;
        // Debug.Log($"expose node / depth = {nodeDepth}");
        IEnumerable<Vector2> turnOwherVisitedPositions;
        IEnumerable<Vector2> OpponentVisitedPositions;
        if (isPlayerTurn){
            turnOwherVisitedPositions = exposeTarget.PlayerVisitedPositions;
            OpponentVisitedPositions = exposeTarget.AIVisitedPositions;
        }
        else{
            turnOwherVisitedPositions = exposeTarget.AIVisitedPositions;
            OpponentVisitedPositions = exposeTarget.PlayerVisitedPositions;
        }
        Vector2 turnOwnerCurrentPosition = turnOwherVisitedPositions.Last();


        //move right
        bool isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.right);
        bool isFacingMapBorder = turnOwnerCurrentPosition.x == (map.Dimensions-1);
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, Vector2.right, isPlayerTurn);
            exposeTarget.Children.Add(newStateNode);
            ExposeState(map, newStateNode, nodeDepth+1, exposingDepth, !isPlayerTurn);
        }

        //move left
        isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.left);
        isFacingMapBorder = turnOwnerCurrentPosition.x == 0;
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, Vector2.left, isPlayerTurn);
            exposeTarget.Children.Add(newStateNode);
            ExposeState(map, newStateNode, nodeDepth+1, exposingDepth, !isPlayerTurn);
        }
        return;

        //move up
        isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.up);
        isFacingMapBorder = turnOwnerCurrentPosition.y == (map.Dimensions-1);
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, Vector2.up, isPlayerTurn);
            exposeTarget.Children.Add(newStateNode);
            ExposeState(map, newStateNode, nodeDepth+1, exposingDepth, !isPlayerTurn);
        }

        //move down
        isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.down);
        isFacingMapBorder = turnOwnerCurrentPosition.y == 0;
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, Vector2.down, isPlayerTurn);
            exposeTarget.Children.Add(newStateNode);
            ExposeState(map, newStateNode, nodeDepth+1, exposingDepth, !isPlayerTurn);
        }
    }
    

    private static MutableStateTreeNode CreateChildMutableStateTreeNode(MutableStateTreeNode parent,
        Vector2 currentPosition, Vector2 direction, bool isPlayerTurn){
        return new MutableStateTreeNode(parent, 
            isPlayerTurn ? parent.PlayerVisitedPositions.DuplicateAndAdd(currentPosition + direction) : parent.PlayerVisitedPositions,
            (!isPlayerTurn) ? parent.AIVisitedPositions.DuplicateAndAdd(currentPosition + direction) : parent.AIVisitedPositions,
            parent.FoodPositions.Contains(currentPosition) 
                ? parent.FoodPositions.DuplicateAndRemove(currentPosition) : parent.FoodPositions
        );
    }


}
