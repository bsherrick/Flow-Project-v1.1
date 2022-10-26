using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowLineGenerator : MonoBehaviour
{
    [HideInInspector] public FlowMain flow;

    [Tooltip("Display debugging messages?")] public bool showDebug;
    [Tooltip("Play sound when an object enters a target (for rhythm mode)?")] public bool targetDebug;

    [Header("Settings (Editor Only):")]
    #region
    [Tooltip("The default theme.")] public bool themeDefault = true;
    [Tooltip("TRUE to enable the space theme.")] public bool themeSpace;
    [Tooltip("The prefab for a line.")] public GameObject line; 
    [Tooltip("The start of the stage (line spawn point).")] public GameObject start;
    [Tooltip("The end of the stage (line kill point).")] public GameObject end;
    #endregion

    [Header("Settings (To be configured by Gamemode):")]
    #region
    //[Tooltip("The map of lines that will be used to generate the level. Spawned descendingly (n-1 to 0)")] [TextArea] public string me_map;
    [Tooltip("In what order (and how many times) should the patches spawn.")] public string me_patchArrangement;
    [Tooltip("The lines in patchA.")] public string me_patchA;
    [Tooltip("The lines in patchB.")] public string me_patchB;
    [Tooltip("The lines in patchC.")] public string me_patchC;
    [Tooltip("The lines in patchD.")] public string me_patchD;
    [Tooltip("The lines in patchE.")] public string me_patchE;
    [Tooltip("The map of lines that will be used to generate the level. Spawned ascendingly (0 to n-1)")] [TextArea] public string me_rhythmMap;
    [Tooltip("The speed by which the line will move.")] public float me_speed;
    [Tooltip("How many times should the lineMap spawn lines.")] public int me_repeat = 1;
    [Tooltip("TRUE if the lineMap should spawn lines endlessly regardless of \"lineMapRepeat\".")] public bool me_endless = false;
    #endregion

    [Header("Status (Per game session):")]
    #region
    [Tooltip("array generated from me_map")] public string[] rhythmMap;
    [Tooltip("array generated from me_patchArrangement & me_patch(A, B, C, D, or E)")] public string[] lineMap;
    [Tooltip("= me_speed")] public float lineSpeed = 5;
    [Tooltip("= me_repeat")] public int lineMapRepeat = 1;
    [Tooltip("= me_endless")] public bool lineEndless = false;
    [Tooltip("For iterating the lines from the lineMap.")] public int index = 0;
    [Tooltip("Spawning point for lines.")] public Vector3 lineSpawnPoint;
    [Tooltip("Distance between the line spawning point and the target board.")] public float distance = 0;
    [Tooltip("Time that it takes for a line to reach the target board given its speed.")] public float travelTime = 0;
    [Tooltip("Amount of delay that the game needs so that the music matches with the lines (for rhythm mode).")] public float delayForced = 0;
    [Tooltip("Arrangment of objects per line.")] public int[] lineArrangment = null;
    //For Rhythm Mode 
    [Tooltip("Check if a particular line has been spawned.")] public bool[] rLinesSpawned = null;
    [Tooltip("Time when the line should be spawned.")] public float[] rTimeToSpawn = null;
    [Tooltip("Time when the line should reach the target.")] float[] rTimeToReach = null;
    [Tooltip("TRUE when the rhythm game starts.")] public bool rStartGame = false;
    [Tooltip("The lines in the current scene.")] public List<GameObject> linesInGame;
    [Tooltip("Amount of lines spawned so far.")] public int linesSpawned = 0;
    //For Algorithm Mode
    [Tooltip("Whether the algorithm uses a seed or is completely Random")] public bool setSeed = false;
    [Tooltip("Seed the algorithm is based on")] public int algorithmSeed;
    [Tooltip("Amount of lines til ship speed increases")] public int speedChangeLines;
    [Tooltip("Max line cooldown amount between lines")] public int maxLineCooldown;
    [Tooltip("Current line cooldown amount between lines")] public int lineCooldown;
    [Tooltip("Whether the algorithm uses adaptive difficulty")] public bool isAdaptiveDifficulty;
    [Tooltip("Amount of Rocks hit till difficulty adapts")] public int adaptiveRocks;
    [Tooltip("Amount of Stars hit till difficulty adapts")] public int adaptiveStars;
    [Tooltip("Total difficulty increase")] public int totalDifficulty;
    [Tooltip("Total rocks allowed to spawn per line")] public int rockDifficulty;
    [Tooltip("Amount of lines til difficulty increases")] public int difficultyChangeLines;
    [Tooltip("Min lines between star spawns")] public int minBetweenStars;
    [Tooltip("Max lines between star spawns")] public int maxBetweenStars;
    [Tooltip("Which line the star will spawn on")] public int starLine;
    [Tooltip("Current line cooldown amount between stars")] public int starCooldown;
    [Tooltip("Stores Dynamic LineSpeed")] public float dynamicLineSpeed;
    #endregion

    public void Initialize()
    {
        flow.audioRhythm1.Stop();
        StopAllCoroutines();

        index = 0;
        delayForced = 0;

        rLinesSpawned = null;
        rTimeToSpawn = null;
        rTimeToReach = null;
        rStartGame = false;

        linesSpawned = 0;

        lineSpeed = me_speed;
        lineMapRepeat = me_repeat;
        lineEndless = me_endless;

        flow.starWarp.sharedMaterial.SetFloat("_WarpSpeed", lineSpeed / 2.5f); //sets the speed of the stars coming in
        lineArrangment = new int[5];
        lineSpawnPoint = start.transform.position + new Vector3(0, 0.5f, 0);
        distance = Vector3.Distance(lineSpawnPoint, end.transform.position + new Vector3(2f, 0.5f, 0));
        travelTime = distance / lineSpeed;

        totalDifficulty = 1;
        rockDifficulty = 1;
    }

    public void StartGame()
    {
        StartCoroutine(BuildGame());
    }

    IEnumerator BuildGame()
    {
        if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm)
        {
            //RHYTHM MODE
            flow.FlowPlayerMovement.playerCharacter.SetActive(false);
            yield return new WaitForSeconds(flow.FlowGameConfig.delayStart);
            SpawnLineRhythm(); //begin spawning lines
        }
        else if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmChop || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmSmooth)
        {
            //ALGORITHM MODE
            if (setSeed)
                Random.InitState(algorithmSeed);

            flow.FlowQuestionHandler.AssignQuestionTriggers();
            flow.FlowQuestionHandler.BeginSpawningQuestions();

            flow.FlowPlayerMovement.targetBoard.GetComponent<MeshRenderer>().enabled = false;
            for (int i = 0; i < flow.FlowPlayerMovement.targets.Length; i++)
            {
                flow.FlowPlayerMovement.targets[i].SetActive(false);
            }

            lineCooldown = 4;
            starLine = Random.Range(minBetweenStars, maxBetweenStars + 1); //Sets which line is a starline
            starCooldown = starLine;

            yield return new WaitForSeconds(flow.AlgorithmValues.delayStart);

            SpawnLineAlgorithm(); //begin spawning lines
        }
        else
        {
            //CLASSIC MODE

            flow.FlowQuestionHandler.AssignQuestionTriggers();
            flow.FlowQuestionHandler.BeginSpawningQuestions();

            flow.FlowPlayerMovement.targetBoard.GetComponent<MeshRenderer>().enabled = false;
            for (int i = 0; i < flow.FlowPlayerMovement.targets.Length; i++)
            {
                flow.FlowPlayerMovement.targets[i].SetActive(false);
            }
            yield return new WaitForSeconds(flow.FlowGameConfig.delayStart);
            ExtractLines();
            SpawnLine(); //begin spawning lines
        }
    }

    void Update()
    {
        if (!flow.gameOn || !rStartGame || index >= rhythmMap.Length)
        {
            return;
        }

        if (rTimeToSpawn[index] <= flow.FlowGameConfig.timeSinceStartOfGame && !rLinesSpawned[index])
        {
            GameObject spawnedLine = Instantiate(line, lineSpawnPoint, Quaternion.identity);
            linesInGame.Add(spawnedLine);

            string debug = "line: "; //only for debugging
            for (int j = 0; j < lineArrangment.Length; j++)
            {
                lineArrangment[j] = int.Parse(rhythmMap[index][j] + "");
                debug += lineArrangment[j] + " "; //only for debugging
            }

            if (showDebug) { Debug.Log(debug); }

            HitLine script = spawnedLine.GetComponent<HitLine>();
            script.LineInit(lineArrangment, flow);

            rLinesSpawned[index] = true;
            index++;
        }
    }

    #region LINE GENERATOR CLASSIC

    /// <summary>
    /// Extract the lines from lineMap to a new lines array (line index is also reset)
    /// </summary>
    void ExtractLines()
    {
        int arraySize = 0;

        #region calculate array size
        for (int i = 0; i < me_patchArrangement.Length; i++)
        {
            if (me_patchArrangement[i] == 'A')
            {
                arraySize += me_patchA.Length;
            }
            else if (me_patchArrangement[i] == 'B')
            {
                arraySize += me_patchB.Length;
            }
            else if (me_patchArrangement[i] == 'C')
            {
                arraySize += me_patchC.Length;
            }
            else if (me_patchArrangement[i] == 'D')
            {
                arraySize += me_patchD.Length;
            }
            else if (me_patchArrangement[i] == 'E')
            {
                arraySize += me_patchE.Length;
            }
            else
            {
                //Do nothing
            }
        }
        arraySize = arraySize / 5;
        #endregion

        lineMap = new string[arraySize];
        string rawLineMap = "";

        #region fill in rawLineMap
        for (int i = me_patchArrangement.Length-1; i >= 0; i--)
        {
            if (me_patchArrangement[i] == 'A')
            {
                for (int j = 0; j < me_patchA.Length; j++)
                {
                    rawLineMap += me_patchA[j];
                }
            }
            else if (me_patchArrangement[i] == 'B')
            {
                for (int j = 0; j < me_patchB.Length; j++)
                {
                    rawLineMap += me_patchB[j];
                }
            }
            else if (me_patchArrangement[i] == 'C')
            {
                for (int j = 0; j < me_patchC.Length; j++)
                {
                    rawLineMap += me_patchC[j];
                }
            }
            else if (me_patchArrangement[i] == 'D')
            {
                for (int j = 0; j < me_patchD.Length; j++)
                {
                    rawLineMap += me_patchD[j];
                }
            }
            else if (me_patchArrangement[i] == 'E')
            {
                for (int j = 0; j < me_patchE.Length; j++)
                {
                    rawLineMap += me_patchE[j];
                }
            }
            else
            {
                //Do nothing
            }
        }
        #endregion

        #region fill in lineMap
        int rawLineMapIndex = 0;
        for (int i = 0; i < arraySize; i++)
        {
            lineMap[i] = "";
            for (int j = 0; j < 5; j++)
            {
                lineMap[i] += rawLineMap[rawLineMapIndex++];
            }
        }
        #endregion

        index = lineMap.Length - 1;
        lineMapRepeat--;
    }

    /// <summary>
    /// Spawns a line.
    /// </summary>
    public void SpawnLine()
    {
        //Debug.Log("Spawn Line Classic");
        if (flow.FlowQuestionHandler.questionInProgress)
        {
            return;
        }

        if (index < 0 && (lineEndless || lineMapRepeat > 0))
        {
            ExtractLines(); //replay lineMap
        }
        else if (index < 0)
        {
            return;
        }

        GameObject spawnedLine = Instantiate(line, lineSpawnPoint, Quaternion.identity);
        linesInGame.Add(spawnedLine);
        linesSpawned++;

        string debug = "line: "; //only for debugging
        for (int j = 0; j < lineArrangment.Length; j++)
        {
            lineArrangment[j] = int.Parse(lineMap[index][j] + "");
            debug += lineArrangment[j] + " "; //only for debugging
        }

        if (showDebug) { Debug.Log(debug); }

        HitLine script = spawnedLine.GetComponent<HitLine>();
        script.LineInit(lineArrangment, flow);
        index--;

        flow.FlowQuestionHandler.TriggerLine(linesSpawned);
    }

    public void ChangeLineSpeed(float newSpeed)
    {
        if (flow.FlowGameConfig.gamePlay != FlowGameConfig.gamePlay_Rhythm)
        {
            lineSpeed = newSpeed;
            flow.starWarp.sharedMaterial.SetFloat("_WarpSpeed", lineSpeed / 2.5f);
        }
    }

    #endregion

    #region LINE GENERATOR ALGORITHM

    /// <summary>
    /// Spawns a line.
    /// </summary>
    public void SpawnLineAlgorithm()
    {
        int maxVarience = 2;
        int minLineCooldown = maxLineCooldown - 5;
        if (minLineCooldown < 2)
            minLineCooldown = 2;

        if (flow.FlowQuestionHandler.questionInProgress)
        {
            return;
        }

        GameObject spawnedLine = Instantiate(line, lineSpawnPoint, Quaternion.identity);
        linesInGame.Add(spawnedLine);
        //linesSpawned++; All lines including blanks

        dynamicLineSpeed = lineSpeed;

        string debug = "line: "; //only for debugging
        if (lineCooldown <= 0) {
            linesSpawned++; //only lines with obsticals

            if(linesSpawned % speedChangeLines == 0) // Line Speed -- Increases Speed by 1, if total lines spawned is a multiple of speedChangeLines
            {
                lineSpeed++;
                ChangeLineSpeedAlgorithm(lineSpeed);
               Debug.Log("LineSpeed = " + lineSpeed);
            }

            if (totalDifficulty < 5)
            {
                if(linesSpawned % 3 == 0)
                    maxVarience = 2;
                else
                    maxVarience = 0;
            }
                
            if (linesSpawned % (difficultyChangeLines + (4 * (totalDifficulty - 1))) == 0) // Difficulty -- Difficulty increases by 1, if total lines spawned is a multiple of difficultyChangeLines
            {
                totalDifficulty++;

                linesSpawned = 1;

                if (totalDifficulty % 2 == 0) //Every 2 difficulty increases, rockDifficulty increases 1
                    rockDifficulty++;

                if (rockDifficulty > 4){ //Rock difficulty caps at 4
                    rockDifficulty = 4;
                    maxVarience = 1;
                }
                if (totalDifficulty % 2 == 0) //Every 3 difficulty increases, lowers the maxLineCooldown by one
                {
                    maxLineCooldown--;
                    if (maxLineCooldown < 2) //maxLineCooldown caps at 2
                        maxLineCooldown = 2;
                }


                Debug.Log("Difficulty = " + totalDifficulty + " | Rocks = " + rockDifficulty);
            }

            int rockCount = 0;
            int rockVarience = Random.Range(0, maxVarience);

            for (rockCount = rockCount + rockVarience; rockCount < rockDifficulty; rockCount++) //Spawns Obstacle Line
            {
                int randomloc = Random.Range(0, lineArrangment.Length);
                int spaceChecks = 0;
                while(lineArrangment[randomloc] == 1 && spaceChecks < 5)
                {
                    spaceChecks++;
                    randomloc++;
                    if (randomloc >= lineArrangment.Length)
                        randomloc = 0;
                }
                lineArrangment[randomloc] = 1;
                debug += lineArrangment[randomloc] + " "; //only for debugging
            }

            /*
            for (int j = 0; j < lineArrangment.Length; j++) //Spawns Obstacle Line
            {
                lineArrangment[j] = 1; //Sets line to be all rocks
                debug += lineArrangment[j] + " "; //only for debugging
            }
            */

            if (starCooldown <= 0)
            {
                lineArrangment[Random.Range(0, 5)] = 2;
                //Debug.Log("Starline");
                starLine = Random.Range(minBetweenStars, maxBetweenStars + 1); //Sets which line is a starline
                starCooldown = starLine;
            }

            starCooldown--;
            if (totalDifficulty < 3)
                lineCooldown = Random.Range(minLineCooldown, 5); //How many free lines between obstical lines (resets cooldown)
            else
                lineCooldown = Random.Range(minLineCooldown, maxLineCooldown); //How many free lines between obstical lines (resets cooldown)

        }
        else //Spawns filler lines
        {
            for (int j = 0; j < lineArrangment.Length; j++)
            {
                lineArrangment[j] = 0; //Sets line to be all blanks
                debug += lineArrangment[j] + " "; //only for debugging
            }
            lineCooldown--; //subtracts from line cooldown
        }

        if (showDebug)
            Debug.Log(debug);

        HitLine script = spawnedLine.GetComponent<HitLine>();
        script.LineInit(lineArrangment, flow);
        //index--;

        flow.FlowQuestionHandler.TriggerLine(linesSpawned);
    }

    public void ChangeLineSpeedAlgorithm(float newSpeed)
    {
        if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmSmooth || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmChop)
        {
            lineSpeed = newSpeed;
            dynamicLineSpeed = lineSpeed;
            flow.starWarp.sharedMaterial.SetFloat("_WarpSpeed", lineSpeed / 2.5f);
        }
    }

    public void AdaptiveDifficultyChange(int type) //Addaptive Diffiulty Change
    {
        if (isAdaptiveDifficulty)
        {
            switch (type)
            {
                case 1: //hit rock

                    //Debug.Log("Adaptive Lower Difficulty");
                    totalDifficulty--;
                    rockDifficulty--;
                    maxLineCooldown++;
                    lineSpeed--;

                    if (rockDifficulty < 2) //Rock difficulty caps at 1
                        rockDifficulty = 2;
                    if (totalDifficulty < 1) //total difficulty caps at 1
                        totalDifficulty = 1;
                    if (maxLineCooldown > 8) //maxLineCooldown caps at 7
                        maxLineCooldown = 8;
                    if (lineSpeed < 4) //lineSpeed caps at 4
                        lineSpeed = 4;

                    break;
                case 2: //hit star

                    //Debug.Log("Adaptive Increase Difficulty");
                    totalDifficulty++;
                    rockDifficulty++;
                    maxBetweenStars++;
                    minBetweenStars++;
                    if (rockDifficulty > 4) //Rock difficulty caps at 4
                        rockDifficulty = 4;

                    break;
            }
        }
    }

    #endregion

    #region LINE GENERATOR RHYTHM

    /// <summary>
    /// Begin spawning lines for the game in rhythm mode.
    /// </summary>
    public void SpawnLineRhythm()
    {
        delayForced = flow.FlowLineGenerator.travelTime + flow.FlowGameConfig.delayStart + 1;

        ExtractMusic();
        int totalHits = 0;
        for (int i = 0; i < rhythmMap.Length; i++)
        {
            for (int j = 0; j < rhythmMap[i].Length; j++)
            {
                if (rhythmMap[i][j].Equals('1')) { totalHits++; }
            }
        }
        flow.FlowGameConfig.hitAmount = totalHits;
        flow.FlowCanvas.inGameRhythmTextChange(1, "HITS: 0/" + totalHits);
        rStartGame = true;
        StartCoroutine(DelayRhythm());
    }

    /// <summary>
    /// Delay for some more time so that the music and the lines are in sync.
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayRhythm()
    {
        yield return new WaitForSeconds(delayForced);
        flow.audioRhythm1.Play(); //todo choose music
    }

    /// <summary>
    /// Extract the lines from rhythmMap
    /// </summary>
    void ExtractMusic()
    {
        string[] splitRaw = me_rhythmMap.Split('|');
        rhythmMap = new string[splitRaw.Length - 1];
        System.Array.Copy(splitRaw, rhythmMap, splitRaw.Length - 1);

        rLinesSpawned = new bool[rhythmMap.Length];
        rTimeToSpawn = new float[rhythmMap.Length];
        rTimeToReach = new float[rhythmMap.Length];

        index = 0;

        string[] temp;
        for (int i = 0; i < rhythmMap.Length; i++)
        {
            temp = rhythmMap[i].Split('@');
            rhythmMap[i] = temp[1];
            rLinesSpawned[i] = false;
            rTimeToSpawn[i] = float.Parse(temp[0]) + flow.FlowGameConfig.timeSinceStartOfGame + flow.FlowGameConfig.delayStart + 1;
            rTimeToReach[i] = rTimeToSpawn[i] + flow.FlowLineGenerator.travelTime;
            //generatedRhythmLines += "[" + i + "]: " + lines[i] + "@" + rTimeToSpawn[i] + "@" + rTimeToReach[i] + "\n";
        }
    }

    #endregion

   
}
