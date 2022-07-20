using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BasketCaseBehaviour : MonoBehaviour
{
    [Header("Colliders")]
    [Tooltip("When the ball enter inside this collider the ball will score as throwed ball")]
    public Collider BasketCaseCollider;
    [Tooltip("When the ball enter inside this collider the ball will score a point")]
    public Collider ScoreCollider;

    [Header("Other Settings")]
    [Tooltip("Will display: [score / throwed]. Can be null")]
    public TextMeshProUGUI ScoreScreen;

    [SerializeField]
    private bool onlyScoreAbove;
    [SerializeField, Range(0, 10)]
    private int secondsBetweenScores = 1;
    [SerializeField, Range(1, 10), Tooltip("If the ball enters from below the user cant score more points before this seconds pass")]
    private int secondsForPreventHacks = 3;

    private AudioSource _audioSource;
    private bool haveScreen;

    private int _throwedBalls;
    private int _scoreBalls;
    private int? previousBallId;
    private bool canScore;


    // Start is called before the first frame update
    void Start()
    {
        if (BasketCaseCollider == null || ScoreCollider == null) Debug.LogError($"({this.gameObject.name}) ERROR: Need BasketCaseCollider and ScoreCollider");
        _audioSource = this.GetComponent<AudioSource>();

        _throwedBalls = 0;
        _scoreBalls = 0;
        canScore = true;
        haveScreen = (ScoreScreen != null);

        ScoreScreen.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void IncreaseThrowedBalls(GameObject ball)
    {
        if (ball.TryGetComponent<Rigidbody>(out _))
        {
            _throwedBalls++;
            previousBallId = ball.GetInstanceID();
            UpdateScreen();
        }
    }

    public void IncreaseScore(GameObject ball)
    {
        Rigidbody ballRb;
        if (canScore && CheckBallId(ball) && ball.TryGetComponent(out ballRb))
        {
            if (!onlyScoreAbove || ballRb.velocity.y < 0)
            {
                _scoreBalls++;
                _audioSource.Play();
                StartCoroutine(PreventOfMoreScores());
                UpdateScreen();
            }
            else if (onlyScoreAbove && ballRb.velocity.y > 0)
                StartCoroutine(PreventOfMoreScores(3));
        }
    }

    public void ForgetBallId()
    {
        previousBallId = null;
    }

    private void UpdateScreen()
    {
        if (haveScreen)
            ScoreScreen.text = $"{_scoreBalls} / {_throwedBalls}"
                + $"\n{(((float)_scoreBalls / (float)_throwedBalls) * 100f).ToString("0.00")}%";
    }

    private bool CheckBallId(GameObject ball)
    {
        var tmpId = ball.GetInstanceID();
        return previousBallId.HasValue && tmpId == previousBallId.Value;
    }

    private IEnumerator PreventOfMoreScores(int seconds = 1)
    {
        canScore = false;
        yield return new WaitForSeconds(1);
        canScore = true;
    }
}
