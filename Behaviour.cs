using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class Behaviour : MonoBehaviour
{
    public Transform ARHEADOBJ;
    public Transform ARLHandOBJ;
    public Transform ARRHandOBJ;
    public Transform VRHEADOBJ;
    public Transform VRLHandOBJ;
    public Transform VRRHandOBJ;

    [System.Serializable]
	public class Stage
	{
        [Tooltip("Name of the stage for organization")]
		public string _stageName;

        [Tooltip("The Stage ID. No other Stages should share the same StageID")]
		public int _stageID;

        [Tooltip("All of the objects tied to the stage. If the stage is deactived the objects will hide. If the stage is activated the objects will be active")]
		public GameObject[] _overridedisableGOs;

        [Tooltip("Do not Edit. This is for READ purposes only and tells you whether an stage is active.")]
        public bool _stageActive;
	}

    [System.Serializable]
    public class Event
    {
        [Tooltip("Name of the event for organization")]
        public string _eventName;

        [Tooltip("Set to true if you want this event to be triggered when the current event ID is set to the event's ID.")]
        public bool _eventTrigger;

        [Tooltip("Keep set to -1 if the event is not triggered by time")]
        public float _eventTime = -1;

        [Tooltip("The Event ID. No other Events should share the same ID.")]
        public int _eventID;

        [Tooltip("Enable if you want audio to happen with the event.")]
        public bool _audioEvent;

        [Tooltip("Enable if you want text to be displayed with the event.")]
        public bool _textEvent;

        [Tooltip("Enable if you want animation to occur when the event occurs")]
        public bool _animationEvent;

        [Tooltip("Do not Edit. This is for READ purposes only and tells you whether an event is occuring.")]
        public bool _eventActive;

        public Animation[] animations;
    }

    [Tooltip("Main Camera View. The Main Camera GameObject should be attached.")]
    public Camera maincam;

    [Tooltip("Current Active Stage. The stage with a stageID corresponding to this will be active and all other stages will be deactivated.")]
	public int _currentStageID;

    [Tooltip("Manage, Add, or Delete Stages")]
	public Stage[] _stages;

    [Tooltip("Current Active Event. The event(s) with a eventID corresponding to this will be active and all other stages will be deactivated.")]
    public int _currentEventID;

    [Tooltip("Manage, Add, or Delete Events")]
    public Event[] _events;

    [Tooltip("Keeps track of current time. Can be set dynamically with the _setTime function and can be paused or started with the _pauseTime function.")]
    public float timeTrack;

    [Tooltip("Runs or stops the timer.Use the _pauseTime function to enable and disable this variable dynamically.")]
    public bool useTimer;

    [System.Serializable]
    public class DATACOLLECT
    {
        [Tooltip("Enable to Start Collect Data. Disable to Stop Collecting Data")]
        public bool collectingData;

        [Tooltip("How often position data is collected. Corresponds directly to frames per second. Higher values = Higher Accuracy and Lower Performance. Lower values =  Lower Accuracy and Higher Performance.")]
        public float TransformSamplingRate;

        [Tooltip("Maximum amount of samples per object tracked. If Elements exceed the Maximum Size and information still is being collected it will drop the oldest data.")]
        public int CollectionSize;

        ////// VR SAMPLING VARIABLES ////////
        public bool CollectVRHEADLOC = false;
        public List<Vector3> VRHEADLOC = new List<Vector3>();
        public Vector3 VRHEADLOCAVG;
        public Vector3 VRHEADLOCSTD;
        public Vector3 VRHEADLOCVAR;

        public bool CollectVRHEADROT = false;
        public List<Vector3> VRHEADROT = new List<Vector3>();
        public Vector3 VRHEADROTAVG;
        public Vector3 VRHEADROTSTD;
        public Vector3 VRHEADROTVAR;

        public bool CollectVRLHANDLOC = false;
        public List<Vector3> VRLHANDLOC = new List<Vector3>();
        public Vector3 VRLHANDLOCAVG;
        public Vector3 VRLHANDLOCSTD;
        public Vector3 VRLHANDLOCVAR;

        public bool CollectVRLHANDROT = false;
        public List<Vector3> VRLHANDROT = new List<Vector3>();
        public Vector3 VRLHANDROTAVG;
        public Vector3 VRLHANDROTSTD;
        public Vector3 VRLHANDROTVAR;

        public bool CollectVRRHANDLOC = false;
        public List<Vector3> VRRHANDLOC = new List<Vector3>();
        public Vector3 VRRHANDLOCAVG;
        public Vector3 VRRHANDLOCSTD;
        public Vector3 VRRHANDLOCVAR;

        public bool CollectVRRHANDROT = false;
        public List<Vector3> VRRHANDROT = new List<Vector3>();
        public Vector3 VRRHANDROTAVG;
        public Vector3 VRRHANDROTSTD;
        public Vector3 VRRHANDROTVAR;

        ////// AR SAMPLING VARIABLES ////////

        public bool CollectARHEADLOC = false;
        public List<Vector3> ARHEADLOC = new List<Vector3>();
        public Vector3 ARHEADLOCAVG;
        public Vector3 ARHEADLOCSTD;
        public Vector3 ARHEADLOCVAR;

        public bool CollectARHEADROT = false;
        public List<Vector3> ARHEADROT = new List<Vector3>();
        public Vector3 ARHEADROTAVG;
        public Vector3 ARHEADROTSTD;
        public Vector3 ARHEADROTVAR;

        public bool CollectARLHANDLOC = false;
        public List<Vector3> ARLHANDLOC = new List<Vector3>();
        public Vector3 ARLHANDLOCAVG;
        public Vector3 ARLHANDLOCSTD;
        public Vector3 ARLHANDLOCVAR;

        public bool CollectARLHANDROT = false;
        public List<Vector3> ARLHANDROT = new List<Vector3>();
        public Vector3 ARLHANDROTAVG;
        public Vector3 ARLHANDROTSTD;
        public Vector3 ARLHANDROTVAR;

        public bool CollectARRHANDLOC = false;
        public List<Vector3> ARRHANDLOC = new List<Vector3>();
        public Vector3 ARRHANDLOCAVG;
        public Vector3 ARRHANDLOCSTD;
        public Vector3 ARRHANDLOCVAR;

        public bool CollectARRHANDROT = false;
        public List<Vector3> ARRHANDROT = new List<Vector3>();
        public Vector3 ARRHANDROTAVG;
        public Vector3 ARRHANDROTSTD;
        public Vector3 ARRHANDROTVAR;
    }

    ///////   Chris' Variables   ////////

    public float FrequencyRange;

    public float audioArcLength;

    public float HearingRange;

    public float colorAcuity;

    public int VisionScore;

    public float ReactionTime;

    public float Balance;

    ////////////////////////////////////

    public DATACOLLECT DataCollect;

    public float GHBBUTTONHEIGHT = 20;

	public GameObject GHBTemplate;

	public Transform GHBParent;

	public Text GHBText;

	public GameObject Mirror;

    public string FileHolderName;


    // Start is called before the first frame update
    void Start()
    {
        maincam = Camera.main;
    }

    public void ScoreData()
    {

    }

    public void SaveData()
	{
		int filenumber = 0;
		filenumber = PlayerPrefs.GetInt("FN", 1);
		PlayerPrefs.SetInt("FN",PlayerPrefs.GetInt("FN",0)+1);
		string path = Application.dataPath +"/"+ FileHolderName +"/HEALTHDATA" + filenumber + ".txt";

        string frequencyskill = "";

        string visualskill = "";

        string colorskill = "";

        string hearingskill = "";

        string balanceskill = "";

        if(Balance < 30.0f)
        {
            if(Balance < 25.0f)
            {
                if(Balance < 10.0f)
                {
                    balanceskill = "Excellent";
                }
                else
                {
                    balanceskill = "Fair";
                }
            }
            else
            {
                balanceskill = "Below Average";
            }
        }
        else
        {
            balanceskill = "Terrible";
        }

        if(HearingRange < 30.0f)
        {
            if(HearingRange < 25.0f)
            {
                if(HearingRange < 10.0f)
                {
                    hearingskill = "Excellent";
                }
                else
                {
                    hearingskill = "Fair";
                }
            }
            else
            {
                hearingskill = "Below Average";
            }
        }
        else
        {
            hearingskill = "Terrible";
        }

        if(colorAcuity < 30.0f)
        {
            if(colorAcuity < 25.0f)
            {
                if(colorAcuity < 10.0f)
                {
                    colorskill = "Great";
                }
                else
                {
                    colorskill = "Fair";
                }
            }
            else
            {
                colorskill = "Below Average";
            }
        }
        else
        {
            colorskill = "Terrible";
        }

        if(VisionScore == 0)
        {
            visualskill = "You need to see a doctor immediately";
        }
        else if (VisionScore == 1)
        {
             visualskill = "20/200";
        }
        else if (VisionScore == 2)
        {
             visualskill = "20/100";
        }
        else if (VisionScore == 3)
        {
             visualskill = "20/70";
        }
        else if (VisionScore == 4)
        {
             visualskill = "20/50";
        }
        else if (VisionScore == 5)
        {
             visualskill = "20/40";
        }
        else if (VisionScore == 6)
        {
             visualskill = "20/30";
        }
        else if (VisionScore == 7)
        {
             visualskill = "20/25";
        }
        else if (VisionScore == 8)
        {
             visualskill = "20/20";
        }
        else if (VisionScore == 9)
        {
             visualskill = "20/15";
        }
        
        
        if(FrequencyRange*HearingRange < 30.0f)
        {
            if(FrequencyRange*HearingRange < 25.0f)
            {
                if(FrequencyRange*HearingRange < 10.0f)
                {
                    frequencyskill = "Excellent Hearing";
                }
                else
                {
                    frequencyskill = "Fair Hearing";
                }
            }
            else
            {
                frequencyskill = "Below Average Hearing";
            }
        }
        else
        {
            frequencyskill = "Terrible Hearing";
        }

		File.WriteAllText(path, "Diagnosis: \n"+ "From the data we've gathered from the lab, I have evaluated your condition and health based on your results from the test\n" +
        "The audio section was designed to measure your hearing level, as well as your audial perception abilities compared to the average human. From your results we can conclude that you have " + frequencyskill +
        "The visual section aimed to simulate a standard eye exam you would take at any doctor's office. From the eye exam we estimated your eye vision to be "+visualskill+
        "Additionally, the color differention test showed your color aptitude to be "+ colorskill +
        "In conclusion, we classify your hearing qualities as " + hearingskill + "and your vision as "+ colorskill + " and your overall balance and stability as " + balanceskill + ". We recommend visiting your doctor for any issues you feel are urgent based on these tests.");  
        //+ 
        //"We recommend you try " + "in order to better your overall health for your future"+
        //"20/20, your vision seems to be perfect. Nice!"+ "Critically poor, if you find any objects in front of you to appear blurry, please contact your nearest optometrist immediately"+
        //"Satisfactory, you don't seem to be at risk for color blindess."+
        //"Concerning, it took you longer to differentiate between the colors than normal. From this we conclude you may be at risk for color blindness." +
        //"you picked up the frequency at a high level, meaning you exhibit excellent hearing!"+
        //"from the frequency you began hearing the pitch, it is estimated you are hard of hearing and recommend getting a serious diagnosis."
        //+"Adequate, although there seemed to be a struggle with audio perception, but nothing too concerning at the moment."
        //+"Based on the data from your tests, we recommend you visit an optometrist for a full comprehensive eye exam since we found you may require glasses."
        //);
	}

    public void LoadPGNData ()
	{
		foreach(Transform child in GHBParent) {
    		Destroy(child.gameObject);
		}
		#if UNITY_EDITOR
 		UnityEditor.AssetDatabase.Refresh();
 		#endif
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath +"/"+FileHolderName);
		FileInfo[] info = dir.GetFiles("*.txt");
		int loc = 0;
		//GameObject clone1 = null;
		foreach (FileInfo f in info)
		{
			GameObject clone1 = Instantiate(GHBTemplate);
			clone1.transform.SetParent(GHBParent);
			clone1.name = f.Name;
			clone1.transform.GetChild(0).gameObject.GetComponent<Text>().text = f.Name;
			clone1.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
			clone1.GetComponent<RectTransform>().offsetMax = new Vector2(1, 1);
			clone1.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
			clone1.transform.GetChild(0).gameObject.name = f.Name;
			int c = loc;
			clone1.GetComponent<Button>().onClick.AddListener(() => SetGHBText(c));
			loc++;
		}
		GHBParent.parent.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, loc*GHBBUTTONHEIGHT);
	}

    public void SetGHBText(int j)
    {
        GHBText.text  = "";
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath +"/"+FileHolderName);
		FileInfo[] info = dir.GetFiles("*.txt");
		StreamReader reader = null; 
 
		if(/*!(j >= 0 && j <= info.Length) &&*/ info[j] != null && info[j].Exists)
		{
			reader = info[j].OpenText();
			//print("entered");
			//print (j);
		}
		if ( reader == null )
		{
			//print (j + " " + info[j].ToString());
   			Debug.Log("info[j].name not found or not readable");
		}
		else
		{
   		// Read each line from the file
		   string txt = "";
   			while ( (txt = reader.ReadLine()) != null )
			{
    			GHBText.text +=txt + " ";
			}
		}
		reader.Close();
    }

    public void ClearData ()
	{
		FileUtil.DeleteFileOrDirectory(Application.dataPath +"/"+FileHolderName);
		Directory.CreateDirectory(Application.dataPath +"/"+FileHolderName);
		PlayerPrefs.SetInt("FN",1);
        GHBText.text = "";
        //foreach(Transform child in GHBParent) {
        //	Destroy(child.gameObject);
        //}
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
 		#endif
	}

    void FixedUpdate()
    {
        if(DataCollect.collectingData == true)
        {
            if(DataCollect.CollectionSize <= DataCollect.ARHEADLOC.Count())
            {
                DataCollect.ARHEADLOC.RemoveAt(0);
                DataCollect.ARHEADROT.RemoveAt(0);
                DataCollect.ARLHANDLOC.RemoveAt(0);
                DataCollect.ARLHANDROT.RemoveAt(0);
                DataCollect.ARRHANDLOC.RemoveAt(0);
                DataCollect.ARRHANDROT.RemoveAt(0);
                DataCollect.VRHEADLOC.RemoveAt(0);
                DataCollect.VRHEADROT.RemoveAt(0);
                DataCollect.VRLHANDLOC.RemoveAt(0);
                DataCollect.VRLHANDROT.RemoveAt(0);
                DataCollect.VRRHANDLOC.RemoveAt(0);
                DataCollect.VRRHANDROT.RemoveAt(0);
            }
            DataCollect.ARHEADLOC.Add(ARHEADOBJ.position);
            DataCollect.ARHEADROT.Add(ARHEADOBJ.rotation.eulerAngles);
            DataCollect.ARLHANDLOC.Add(ARLHandOBJ.position);
            DataCollect.ARLHANDROT.Add(ARLHandOBJ.rotation.eulerAngles);
            DataCollect.ARRHANDLOC.Add(ARRHandOBJ.position);
            DataCollect.ARRHANDROT.Add(ARRHandOBJ.rotation.eulerAngles);
            DataCollect.VRHEADLOC.Add(VRHEADOBJ.position);
            DataCollect.VRHEADROT.Add(VRHEADOBJ.rotation.eulerAngles);
            DataCollect.VRLHANDLOC.Add(VRLHandOBJ.position);
            DataCollect.VRLHANDROT.Add(VRLHandOBJ.rotation.eulerAngles);
            DataCollect.VRRHANDLOC.Add(VRRHandOBJ.position);
            DataCollect.VRRHANDROT.Add(VRRHandOBJ.rotation.eulerAngles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Stage stage in _stages)
		{
			if(stage._stageID != _currentStageID)
			{
                stage._stageActive = true;

				foreach(GameObject _overridedisableGO in stage._overridedisableGOs)
				{   
                    _overridedisableGO.SetActive(false);
				}
			}
			else
			{
                stage._stageActive = true;

				foreach(GameObject _overridedisableGO in stage._overridedisableGOs)
				{
					_overridedisableGO.SetActive(true);
				}
			}
		}
        foreach(Event _event in _events)
		{
			if(_event._eventID == _currentEventID)
			{
                if(_event._eventActive != true)
                {
                    if(_event._audioEvent == true)
                    {

                    }
                    if(_event._textEvent == true)
                    {

                    }
                    if(_event._animationEvent == true)
                    {
                        foreach(Animation anim in _event.animations)
                        {
                            anim.Play();
                        }
                    }
                }
				_event._eventActive = true;
			}
			else
			{
				_event._eventActive = false;
			}
		}
        if(useTimer == true)
        {
            timeTrack += Time.deltaTime;
        }
    }

    public void _goToStage (int _newStageID)
	{
		_currentStageID = _newStageID;
	}
    public void _setTime (float _time)
    {
        timeTrack = _time;
    }
    public void _pauseTime (bool pause)
    {
        useTimer = pause;
    }
    public void _setEvent (int eventID)
    {
        _currentEventID = eventID;
    }

    public void ClearFileData ()
    {
        DataCollect.ARHEADLOC.Clear();
        DataCollect.ARHEADROT.Clear();
        DataCollect.ARLHANDLOC.Clear();
        DataCollect.ARLHANDROT.Clear();
        DataCollect.ARRHANDLOC.Clear();
        DataCollect.ARRHANDROT.Clear();
        DataCollect.VRHEADLOC.Clear();
        DataCollect.VRHEADROT.Clear();
        DataCollect.VRLHANDLOC.Clear();
        DataCollect.VRLHANDROT.Clear();
        DataCollect.VRRHANDLOC.Clear();
        DataCollect.VRRHANDROT.Clear();
    }

    public void CalculateSTATISTICS ()
    {
        if(DataCollect.CollectARHEADLOC == true)
        {
            //ARHEAD LOC
            DataCollect.ARHEADLOCAVG = new Vector3(DataCollect.ARHEADLOC.Average(x=>x.x),
            DataCollect.ARHEADLOC.Average(x=>x.y),
            DataCollect.ARHEADLOC.Average(x=>x.z));
            DataCollect.ARHEADLOCSTD = new Vector3(DataCollect.ARHEADLOC.Sum(x=>(float)Math.Pow(x.x-DataCollect.ARHEADLOCAVG.x,2.0f)),
            DataCollect.ARHEADLOC.Sum(x=>(float)Math.Pow(x.y-DataCollect.ARHEADLOCAVG.y,2.0f)),
            DataCollect.ARHEADLOC.Sum(x=>(float)Math.Pow(x.z-DataCollect.ARHEADLOCAVG.z,2.0f)));
            DataCollect.ARHEADLOCVAR = new Vector3((float)Math.Pow(DataCollect.ARHEADLOCSTD.x,2), (float)Math.Pow(DataCollect.ARHEADLOCSTD.y,2), (float)Math.Pow(DataCollect.ARHEADLOCSTD.z,2));

            //ARHEAD ROT
            DataCollect.ARHEADROTAVG = new Vector3(DataCollect.ARHEADROT.Average(x=>x.x),
            DataCollect.ARHEADROT.Average(x=>x.y),
            DataCollect.ARHEADROT.Average(x=>x.z));
            DataCollect.ARHEADROTSTD = new Vector3(DataCollect.ARHEADROT.Sum(x=>(float)Math.Pow(x.x-DataCollect.ARHEADROTAVG.x,2.0f)),
            DataCollect.ARHEADROT.Sum(x=>(float)Math.Pow(x.y-DataCollect.ARHEADROTAVG.y,2.0f)),
            DataCollect.ARHEADROT.Sum(x=>(float)Math.Pow(x.z-DataCollect.ARHEADROTAVG.z,2.0f)));
            DataCollect.ARHEADROTVAR = new Vector3((float)Math.Pow(DataCollect.ARHEADROTSTD.x,2), (float)Math.Pow(DataCollect.ARHEADROTSTD.y,2), (float)Math.Pow(DataCollect.ARHEADROTSTD.z,2));

            //ARLHAND LOC
            DataCollect.ARLHANDLOCAVG = new Vector3(DataCollect.ARLHANDLOC.Average(x=>x.x),
            DataCollect.ARLHANDLOC.Average(x=>x.y),
            DataCollect.ARLHANDLOC.Average(x=>x.z));
            DataCollect.ARLHANDLOCSTD = new Vector3(DataCollect.ARLHANDLOC.Sum(x=>(float)Math.Pow(x.x-DataCollect.ARLHANDLOCAVG.x,2.0f)),
            DataCollect.ARLHANDLOC.Sum(x=>(float)Math.Pow(x.y-DataCollect.ARLHANDLOCAVG.y,2.0f)),
            DataCollect.ARLHANDLOC.Sum(x=>(float)Math.Pow(x.z-DataCollect.ARLHANDLOCAVG.z,2.0f)));
            DataCollect.ARLHANDLOCVAR = new Vector3((float)Math.Pow(DataCollect.ARLHANDLOCSTD.x,2), (float)Math.Pow(DataCollect.ARLHANDLOCSTD.y,2), (float)Math.Pow(DataCollect.ARLHANDLOCSTD.z,2));

            //ARLHAND ROT
            DataCollect.ARLHANDROTAVG = new Vector3(DataCollect.ARLHANDROT.Average(x=>x.x),
            DataCollect.ARLHANDROT.Average(x=>x.y),
            DataCollect.ARLHANDROT.Average(x=>x.z));
            DataCollect.ARLHANDROTSTD = new Vector3(DataCollect.ARLHANDROT.Sum(x=>(float)Math.Pow(x.x-DataCollect.ARLHANDROTAVG.x,2.0f)),
            DataCollect.ARLHANDROT.Sum(x=>(float)Math.Pow(x.y-DataCollect.ARLHANDROTAVG.y,2.0f)),
            DataCollect.ARLHANDROT.Sum(x=>(float)Math.Pow(x.z-DataCollect.ARLHANDROTAVG.z,2.0f)));
            DataCollect.ARLHANDROTVAR = new Vector3((float)Math.Pow(DataCollect.ARLHANDROTSTD.x,2), (float)Math.Pow(DataCollect.ARLHANDROTSTD.y,2), (float)Math.Pow(DataCollect.ARLHANDROTSTD.z,2));

            //ARRHAND LOC
            DataCollect.ARRHANDLOCAVG = new Vector3(DataCollect.ARRHANDLOC.Average(x=>x.x),
            DataCollect.ARRHANDLOC.Average(x=>x.y),
            DataCollect.ARRHANDLOC.Average(x=>x.z));
            DataCollect.ARRHANDLOCSTD = new Vector3(DataCollect.ARRHANDLOC.Sum(x=>(float)Math.Pow(x.x-DataCollect.ARRHANDLOCAVG.x,2.0f)),
            DataCollect.ARRHANDLOC.Sum(x=>(float)Math.Pow(x.y-DataCollect.ARRHANDLOCAVG.y,2.0f)),
            DataCollect.ARRHANDLOC.Sum(x=>(float)Math.Pow(x.z-DataCollect.ARRHANDLOCAVG.z,2.0f)));
            DataCollect.ARRHANDLOCVAR = new Vector3((float)Math.Pow(DataCollect.ARRHANDLOCSTD.x,2), (float)Math.Pow(DataCollect.ARRHANDLOCSTD.y,2), (float)Math.Pow(DataCollect.ARRHANDLOCSTD.z,2));

            //ARRHAND ROT
            DataCollect.ARRHANDROTAVG = new Vector3(DataCollect.ARRHANDROT.Average(x=>x.x),
            DataCollect.ARRHANDROT.Average(x=>x.y),
            DataCollect.ARRHANDROT.Average(x=>x.z));
            DataCollect.ARRHANDROTSTD = new Vector3(DataCollect.ARRHANDROT.Sum(x=>(float)Math.Pow(x.x-DataCollect.ARRHANDROTAVG.x,2.0f)),
            DataCollect.ARRHANDROT.Sum(x=>(float)Math.Pow(x.y-DataCollect.ARRHANDROTAVG.y,2.0f)),
            DataCollect.ARRHANDROT.Sum(x=>(float)Math.Pow(x.z-DataCollect.ARRHANDROTAVG.z,2.0f)));
            DataCollect.ARRHANDROTVAR = new Vector3((float)Math.Pow(DataCollect.ARRHANDROTSTD.x,2), (float)Math.Pow(DataCollect.ARRHANDROTSTD.y,2), (float)Math.Pow(DataCollect.ARRHANDROTSTD.z,2));

            //VRHEAD LOC
            DataCollect.VRHEADLOCAVG = new Vector3(DataCollect.VRHEADLOC.Average(x=>x.x),
            DataCollect.VRHEADLOC.Average(x=>x.y),
            DataCollect.VRHEADLOC.Average(x=>x.z));
            DataCollect.VRHEADLOCSTD = new Vector3(DataCollect.VRHEADLOC.Sum(x=>(float)Math.Pow(x.x-DataCollect.VRHEADLOCAVG.x,2.0f)),
            DataCollect.VRHEADLOC.Sum(x=>(float)Math.Pow(x.y-DataCollect.VRHEADLOCAVG.y,2.0f)),
            DataCollect.VRHEADLOC.Sum(x=>(float)Math.Pow(x.z-DataCollect.VRHEADLOCAVG.z,2.0f)));
            DataCollect.VRHEADLOCVAR = new Vector3((float)Math.Pow(DataCollect.VRHEADLOCSTD.x,2), (float)Math.Pow(DataCollect.VRHEADLOCSTD.y,2), (float)Math.Pow(DataCollect.VRHEADLOCSTD.z,2));

            //VRHEAD ROT
            DataCollect.VRHEADROTAVG = new Vector3(DataCollect.VRHEADROT.Average(x=>x.x),
            DataCollect.VRHEADROT.Average(x=>x.y),
            DataCollect.VRHEADROT.Average(x=>x.z));
            DataCollect.VRHEADROTSTD = new Vector3(DataCollect.VRHEADROT.Sum(x=>(float)Math.Pow(x.x-DataCollect.VRHEADROTAVG.x,2.0f)),
            DataCollect.VRHEADROT.Sum(x=>(float)Math.Pow(x.y-DataCollect.VRHEADROTAVG.y,2.0f)),
            DataCollect.VRHEADROT.Sum(x=>(float)Math.Pow(x.z-DataCollect.VRHEADROTAVG.z,2.0f)));
            DataCollect.VRHEADROTVAR = new Vector3((float)Math.Pow(DataCollect.VRHEADROTSTD.x,2), (float)Math.Pow(DataCollect.VRHEADROTSTD.y,2), (float)Math.Pow(DataCollect.VRHEADROTSTD.z,2));

            //VRLHAND LOC
            DataCollect.VRLHANDLOCAVG = new Vector3(DataCollect.VRLHANDLOC.Average(x=>x.x),
            DataCollect.VRLHANDLOC.Average(x=>x.y),
            DataCollect.VRLHANDLOC.Average(x=>x.z));
            DataCollect.VRLHANDLOCSTD = new Vector3(DataCollect.VRLHANDLOC.Sum(x=>(float)Math.Pow(x.x-DataCollect.VRLHANDLOCAVG.x,2.0f)),
            DataCollect.VRLHANDLOC.Sum(x=>(float)Math.Pow(x.y-DataCollect.VRLHANDLOCAVG.y,2.0f)),
            DataCollect.VRLHANDLOC.Sum(x=>(float)Math.Pow(x.z-DataCollect.VRLHANDLOCAVG.z,2.0f)));
            DataCollect.VRLHANDLOCVAR = new Vector3((float)Math.Pow(DataCollect.VRLHANDLOCSTD.x,2), (float)Math.Pow(DataCollect.VRLHANDLOCSTD.y,2), (float)Math.Pow(DataCollect.VRLHANDLOCSTD.z,2));

            //VRLHAND ROT
            DataCollect.VRLHANDROTAVG = new Vector3(DataCollect.VRLHANDROT.Average(x=>x.x),
            DataCollect.VRLHANDROT.Average(x=>x.y),
            DataCollect.VRLHANDROT.Average(x=>x.z));
            DataCollect.VRLHANDROTSTD = new Vector3(DataCollect.VRLHANDROT.Sum(x=>(float)Math.Pow(x.x-DataCollect.VRLHANDROTAVG.x,2.0f)),
            DataCollect.VRLHANDROT.Sum(x=>(float)Math.Pow(x.y-DataCollect.VRLHANDROTAVG.y,2.0f)),
            DataCollect.VRLHANDROT.Sum(x=>(float)Math.Pow(x.z-DataCollect.VRLHANDROTAVG.z,2.0f)));
            DataCollect.VRLHANDROTVAR = new Vector3((float)Math.Pow(DataCollect.VRLHANDROTSTD.x,2), (float)Math.Pow(DataCollect.VRLHANDROTSTD.y,2), (float)Math.Pow(DataCollect.VRLHANDROTSTD.z,2));

            //VRRHAND LOC
            DataCollect.VRRHANDLOCAVG = new Vector3(DataCollect.VRRHANDLOC.Average(x=>x.x),
            DataCollect.VRRHANDLOC.Average(x=>x.y),
            DataCollect.VRRHANDLOC.Average(x=>x.z));
            DataCollect.VRRHANDLOCSTD = new Vector3(DataCollect.VRRHANDLOC.Sum(x=>(float)Math.Pow(x.x-DataCollect.VRRHANDLOCAVG.x,2.0f)),
            DataCollect.VRRHANDLOC.Sum(x=>(float)Math.Pow(x.y-DataCollect.VRRHANDLOCAVG.y,2.0f)),
            DataCollect.VRRHANDLOC.Sum(x=>(float)Math.Pow(x.z-DataCollect.VRRHANDLOCAVG.z,2.0f)));
            DataCollect.VRRHANDLOCVAR = new Vector3((float)Math.Pow(DataCollect.VRRHANDLOCSTD.x,2), (float)Math.Pow(DataCollect.VRRHANDLOCSTD.y,2), (float)Math.Pow(DataCollect.VRRHANDLOCSTD.z,2));

            //VRRHAND ROT
            DataCollect.VRRHANDROTAVG = new Vector3(DataCollect.VRRHANDROT.Average(x=>x.x),
            DataCollect.VRRHANDROT.Average(x=>x.y),
            DataCollect.VRRHANDROT.Average(x=>x.z));
            DataCollect.VRRHANDROTSTD = new Vector3(DataCollect.VRRHANDROT.Sum(x=>(float)Math.Pow(x.x-DataCollect.VRRHANDROTAVG.x,2.0f)),
            DataCollect.VRRHANDROT.Sum(x=>(float)Math.Pow(x.y-DataCollect.VRRHANDROTAVG.y,2.0f)),
            DataCollect.VRRHANDROT.Sum(x=>(float)Math.Pow(x.z-DataCollect.VRRHANDROTAVG.z,2.0f)));
            DataCollect.VRRHANDROTVAR = new Vector3((float)Math.Pow(DataCollect.VRRHANDROTSTD.x,2), (float)Math.Pow(DataCollect.VRRHANDROTSTD.y,2), (float)Math.Pow(DataCollect.VRRHANDROTSTD.z,2));        
        
        }
    }
}