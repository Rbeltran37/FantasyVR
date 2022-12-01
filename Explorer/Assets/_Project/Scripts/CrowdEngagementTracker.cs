using UnityEngine;

public class CrowdEngagementTracker : MonoBehaviour
{
    [SerializeField] private float totalPoints;
    [SerializeField] private float totalTimeEngagedWithCrowd = 0f;
    [SerializeField] private float pointsModifier = 0f;

    //[SerializeField] private CrowdInteractor crowdInteractor;
    private bool _hasRecievedTime = false;
    private float _startTime = 0f;
    
    public void InitiateCounter(bool isengaged)
    {
        if (isengaged)
        {
            StartTime();
        }
        else
        {
            GetTotalTime();
        }
    }

    private void StartTime()
    {
        if (!_hasRecievedTime)
        {
            _startTime = Time.time;
            _hasRecievedTime = true;
        }
    }

    private void GetTotalTime()
    {
        if (_hasRecievedTime)
        {
            totalTimeEngagedWithCrowd = Time.time - _startTime;
            _hasRecievedTime = false;
            ConvertTimeToPoints(totalTimeEngagedWithCrowd);
        }
    }

    //This will  most likely have to be rewritten depending on how complex and how we integrate
    //this into the game and scoring system. 
    public void ConvertTimeToPoints(float totalTime)
    {
        totalPoints = totalTime * pointsModifier;
    }

    private void RemovePoints(int pointsToRemove)
    {
        totalPoints -= pointsToRemove;
    }
    
    private void AddPoints(int pointsToAdd)
    { 
        totalPoints += pointsToAdd;
        _startTime = Time.time;
    }

}
