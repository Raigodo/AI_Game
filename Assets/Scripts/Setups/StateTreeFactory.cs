using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateTreeFactory
{
    public static StateTreeMutable Create(MapEntity map, SessionConf conf){
        var tree = new StateTreeMutable(map, conf);
        ExposeTree(map, tree.CurrentStateNode, conf.MaxTurnCount, conf.IsPlayerStarting);
        return tree;
    }

    
    private static void ExposeTree(MapEntity map, MutableStateTreeNode treeRoot, int maxDepth, bool isPlayerStarting){
        foreach (var stateNode in treeRoot.Children){
            ExposeState(map, stateNode, 0, maxDepth, isPlayerStarting);
        }
    }

    private static void ExposeState(MapEntity map, MutableStateTreeNode exposeTarget, int parentDepth, int maxDepth, bool isPlayerTurn){
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
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, isPlayerTurn);
            ExposeState(map, newStateNode, parentDepth+1, maxDepth, !isPlayerTurn);
        }

        //move left
        isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.left);
        isFacingMapBorder = turnOwnerCurrentPosition.x == 0;
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, isPlayerTurn);
            ExposeState(map, newStateNode, parentDepth+1, maxDepth, !isPlayerTurn);
        }

        //move up
        isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.up);
        isFacingMapBorder = turnOwnerCurrentPosition.y == (map.Dimensions-1);
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, isPlayerTurn);
            ExposeState(map, newStateNode, parentDepth+1, maxDepth, !isPlayerTurn);
        }

        //move down
        isCellNotVisitedInDirection = !turnOwherVisitedPositions.Contains(turnOwnerCurrentPosition + Vector2.down);
        isFacingMapBorder = turnOwnerCurrentPosition.y == 0;
        if (isCellNotVisitedInDirection && !isFacingMapBorder){
            var newStateNode = CreateChildMutableStateTreeNode(exposeTarget, turnOwnerCurrentPosition, isPlayerTurn);
            ExposeState(map, newStateNode, parentDepth+1, maxDepth, !isPlayerTurn);
        }
    }
    

    private static MutableStateTreeNode CreateChildMutableStateTreeNode(MutableStateTreeNode parent, Vector2 currentPosition, bool isPlayerTurn){
        return new MutableStateTreeNode(parent, 
            isPlayerTurn ? parent.PlayerVisitedPositions.DuplicateAndAdd(currentPosition + Vector2.right) : parent.PlayerVisitedPositions,
            (!isPlayerTurn) ? parent.AIVisitedPositions.DuplicateAndAdd(currentPosition + Vector2.right) : parent.AIVisitedPositions,
            parent.FoodPositions.Contains(currentPosition) 
                ? parent.FoodPositions.DuplicateAndRemove(currentPosition) : parent.FoodPositions
        );
    }
}
