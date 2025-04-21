using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternController : MonoBehaviour
{
    [System.Serializable]
    public class PatternData
    {
        public float moveSpeed = 10f;
        public int burstCount = 3;
        public float burstDelay = 1f;
        public float restTime = 2f;
        public int projectilesPerBurst = 5;
        public float angleSpread = 45f;
        public float spawnDistance = 0.2f;
        public bool stagger = false;
        public bool oscillate = false;
    }

    [SerializeField] private BossShooter bossShooter;
    [SerializeField] private List<PatternData> patternCycle = new List<PatternData>();
    [SerializeField] private float timeBetweenPatterns = 10f;

    private int currentPatternIndex = 0;
    private float timer = 0f;

    private void Start()
    {
        // OPTIONAL: auto-fill 3 preset patterns if none are set
        if (patternCycle == null || patternCycle.Count == 0)
        {
            patternCycle = new List<PatternData>() {
                new PatternData {
                    moveSpeed = 18f, burstCount = 2, burstDelay = 0.5f, restTime = 2f,
                    projectilesPerBurst = 3, angleSpread = 10f, spawnDistance = 1.5f,
                    stagger = false, oscillate = false
                },
                new PatternData {
                    moveSpeed = 7f, burstCount = 2, burstDelay = 1.2f, restTime = 2.5f,
                    projectilesPerBurst = 9, angleSpread = 120f, spawnDistance = 1.5f,
                    stagger = false, oscillate = false
                },
                new PatternData {
                    moveSpeed = 10f, burstCount = 4, burstDelay = 1f, restTime = 3f,
                    projectilesPerBurst = 6, angleSpread = 90f, spawnDistance = 1.5f,
                    stagger = true, oscillate = true
                }
            };
        }

        ApplyPattern(patternCycle[0]);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenPatterns)
        {
            timer = 0f;
            CyclePattern();
        }
    }

    private void CyclePattern()
    {
        currentPatternIndex = (currentPatternIndex + 1) % patternCycle.Count;
        ApplyPattern(patternCycle[currentPatternIndex]);
    }

    private void ApplyPattern(PatternData data)
    {
        bossShooter.ConfigurePattern(
            data.moveSpeed,
            data.burstCount,
            data.burstDelay,
            data.restTime,
            data.projectilesPerBurst,
            data.angleSpread,
            data.spawnDistance,
            data.stagger,
            data.oscillate
        );
    }
}
