using UnityEngine;

/// <summary>
/// The actor is movng to cover.
/// </summary>
public class AITakingCoverState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
		//if (!input.targetInRange)
  //      {
		//	return new AIMovingToTargetState();
		//}

		// what if the player moved behind something though?
		if (input.fullyInCover)
        {
			return new AIInCoverState();
        }

        return this;
    }

	protected override void _StateUpdate()
	{
		// make enemy not go to cover if it would bring them out of range of the target?
		Collider closestCover = null;
		// cast overlap sphere with radius = range to see if target is possibly in range
		Collider[] hitColliders = Physics.OverlapSphere(_controller.transform.position, _controller.GetAIData().coverSearchRadius, LayerMask.GetMask("HouseAndFurniture"), QueryTriggerInteraction.Collide);
		foreach (Collider c in hitColliders)
		{
			if (c.CompareTag("Cover"))
			{
				if (closestCover == null)
				{
					closestCover = c;
				}
				else if ((_controller.transform.position - c.ClosestPoint(_controller.transform.position)).magnitude < (_controller.transform.position - closestCover.ClosestPoint(_controller.transform.position)).magnitude)
				{
					closestCover = c;
				}
			}
		}

		if (closestCover != null)
		{
			if (!_controller.fullyInCover)
			{

				// if it's a "Floor" cover (not "Wall"), then it's horizontal cover.
				if (closestCover.GetComponent<Cover>().coverType == Cover.CoverType.Floor)
				{
					//inHorizontalCover = true;
				}

				Vector3[] arr = new Vector3[4];
				arr[0] = closestCover.transform.position - Vector3.right * 50f;
				arr[1] = closestCover.transform.position + Vector3.right * 50f;
				arr[2] = closestCover.transform.position - Vector3.up * 50f;
				arr[3] = closestCover.transform.position + Vector3.up * 50f;

				Vector3 targetMovePosition = arr[0];
				for (int i = 1; i < arr.Length; i++)
				{
					if ((_controller.GetTarget().transform.position - arr[i]).magnitude > (_controller.GetTarget().transform.position - targetMovePosition).magnitude)
					{
						targetMovePosition = arr[i];
					}
				}

				_controller.GetActor().Move(closestCover.ClosestPoint(targetMovePosition));
				//takingCover = true;
			}
			else if (_controller.fullyInCover)
			{
				_controller.GetActor().Move(closestCover.ClosestPoint(_controller.transform.position));
			}
		}
	}
}
