using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;
using UnityEngine.Video;
using UnityEditor;

public class testing : MonoBehaviour
{
    //PUBLIC 
    // this is the floor. I used it to test flashing on hint
    //public GameObject cube;


    //which scenario
    public bool tire = true;
    public bool battery = false;
    

    //Private
    //keyword recognizer
    private KeywordRecognizer keywordRecognizer;
    //dictionary of keywords and functions when recognized
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    //index for video clip array
    private int videoIndex = 0;
    //set to video system video player
    private VideoPlayer player; 


    //hint audio source. will be the mechanic
    public AudioSource test;


    //array of videoclips for tutorials
    public VideoClip[] videoClips;

    //audio arrays for hints etc. 
    public AudioClip[] audioClipsForTireChange;
    public AudioClip[] audioClipsForBatJump;
    public AudioClip[] hintsForTireChange;
    public AudioClip[] hintsForBatteryJump;


    //                                   Steps for tire change scenario
    public bool speakTire = false;
    //safe Location
    public bool tirestep1 = true;
    //loosen the lug nuts
    public bool tirestep2 = false;
    //jack placement
    public bool tirestep3 = false;
    //raise jack
    public bool tirestep4 = false;
    //undo the lug nuts
    public bool tirestep5 = false;
    //remove bad tire
    public bool tirestep6 = false;
    //mount spare tire
    public bool tirestep7 = false;
    //tighten the lug nuts by hand
    public bool tirestep8 = false;
    //lower the vehicle
    public bool tirestep9 = false;
    //tighten lug nuts with wrench
    public bool tirestep10 = false;


    //                                    Steps for battery jump scenario
    public bool speakBat = false;
    //safe Location
    public bool batterystep1 = true;
    // pop hood
    public bool batterystep2 = false;
    // red to postive of dead
    public bool batterystep3 = false;
    // red to positive of good
    public bool batterystep4 = false;
    // ground to negative of good
    public bool batterystep5 = false;
    // ground to negative of dead
    public bool batterystep6 = false;
    // crank working then dead car
    public bool batterystep7 = false;
    // remove ground to negative of dead
    public bool batterystep8 = false;
    // remove ground to negative of good
    public bool batterystep9 = false;
    // remove red to positive of good
    public bool batterystep10 = false;
    // remove red to postive of dead
    //public bool batterystep11 = false;


    /*
    //finish in general
    public bool step11 = false;
    //safe Location
    public bool step12 = false;
    //safe Location
    public bool step13 = false;
    //safe Location
    public bool step14 = false;
    //safe Location
    public bool step15 = false;
    */

    // Start is called before the first frame update
    void Start()
    {

        if (tire)
        {
            speakTire = true;
        }

        if (battery)
        {
            speakBat = true;
        }
        //set player to video player component of video system object
        player=GameObject.Find("video system").GetComponent<VideoPlayer>();
        //go ahead and set the first clip
        player.clip = videoClips[0];

        /*test.clip = audioClipsForTireChange[0];
        test.Play();
        */

        //StartCoroutine(ExampleCoroutine());


        //cube = GameObject.Find("Cube");

        actions.Add("help", provideHint);
        actions.Add("hint", provideHint);
        actions.Add("Check", Check);
        actions.Add("next", setNextClip);
        actions.Add("where", provideHintDetailed);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += recognizedSpeech;
        keywordRecognizer.Start();

  //      halo = GetComponent<Halo>();
        
    }

    // Update is called once per frame
    void Update()
    {
        soundOnComplete();
        stepCheck();
    }

    public void soundOnComplete()
    {
       /* //safe Location
        public bool tirestep1 = true;
        //loosen the lug nuts
        public bool tirestep2 = false;
        //jack placement
        public bool tirestep3 = false;
        //raise jack
        public bool tirestep4 = false;
        //undo the lug nuts
        public bool tirestep5 = false;
        //remove bad tire
        public bool tirestep6 = false;
        //mount spare tire
        public bool tirestep7 = false;
        //tighten the lug nuts by hand
        public bool tirestep8 = false;
        //lower the vehicle
        public bool tirestep9 = false;
        //tighten lug nuts with wrench
        public bool tirestep10 = false;
        */

        if (speakTire)
        {
            int whatSpeak = findStepTire();
            if (0 <= whatSpeak && whatSpeak < 10)
            {
                GameObject[] objects;
                objects = getOjectsTire(whatSpeak);

                test.clip = audioClipsForTireChange[whatSpeak];
                test.Play();
                speakTire = !speakTire;

                StartCoroutine(Flashing(objects));
            }
       }
        if (speakBat)
        {
            int whatSpeak = findStepBat();
            if (0 <= whatSpeak && whatSpeak < 11)
            {
                GameObject[] objects;
                objects = getOjectsBat(whatSpeak);

                test.clip = audioClipsForBatJump[whatSpeak];
                test.Play();
                speakBat= !speakBat;

                StartCoroutine(Flashing(objects));
            }
 
        }
    }

    public int findStepTire()
    {
        if (tirestep1)
        {
            if (tirestep2)
            {
                if (tirestep3)
                {
                    if (tirestep4)
                    {
                        if (tirestep5)
                        {
                            if (tirestep6)
                            {
                                if (tirestep7)
                                {
                                    if (tirestep8)
                                    {
                                        if (tirestep9)
                                        {
                                            if (tirestep10)
                                            {
                                                return 9;
                                            }
                                            return 8;
                                        }
                                        return 7;
                                    }
                                    return 6;
                                }
                                return 5;
                            }
                            return 4;
                        }
                        return 3;
                    }
                    return 2;
                }
                return 1;
            }
            return 0;
        }
        return -1;
    }

    GameObject[] getOjectsTire(int x)
    {
        GameObject[] final;
        List<GameObject> ret=new List<GameObject>();
        if (x == 1)
        {
            ret.Add(GameObject.Find("halo lug 1"));
            ret.Add(GameObject.Find("halo lug 2"));
            ret.Add(GameObject.Find("halo lug 3"));
            ret.Add(GameObject.Find("halo lug 4"));
            ret.Add(GameObject.Find("halo lug 5"));
            ret.Add(GameObject.Find("Lug Wrench"));

        }else if(x == 2)
        {
            ret.Add(GameObject.Find("jack"));

        }else if(x == 3)
        {
            ret.Add(GameObject.Find("jack"));
            ret.Add(GameObject.Find("Lug Wrench"));

        }else if(x == 4)
        {
            ret.Add(GameObject.Find("Lug Wrench"));
            ret.Add(GameObject.Find("halo lug 1"));
            ret.Add(GameObject.Find("halo lug 2"));
            ret.Add(GameObject.Find("halo lug 3"));
            ret.Add(GameObject.Find("halo lug 4"));
            ret.Add(GameObject.Find("halo lug 5"));

        }else if(x == 5)
        {
            ret.Add(GameObject.Find("bad tire halo"));

        }else if(x == 6)
        {
            ret.Add(GameObject.Find("good tire"));

        }else if(x == 7)
        {
            ret.Add(GameObject.Find("halo lug 1"));
            ret.Add(GameObject.Find("halo lug 2"));
            ret.Add(GameObject.Find("halo lug 3"));
            ret.Add(GameObject.Find("halo lug 4"));
            ret.Add(GameObject.Find("halo lug 5"));

        }else if(x == 8)
        {
            ret.Add(GameObject.Find("jack"));

        }else if(x == 9)
        {
            ret.Add(GameObject.Find("halo lug 1"));
            ret.Add(GameObject.Find("halo lug 2"));
            ret.Add(GameObject.Find("halo lug 3"));
            ret.Add(GameObject.Find("halo lug 4"));
            ret.Add(GameObject.Find("halo lug 5"));

            ret.Add(GameObject.Find("Lug Wrench"));

        }else if(x == 10)
        {

        }
        final = ret.ToArray();

        return final;
    }



    public int findStepBat()
    {
        if (batterystep1)
        {
            if (batterystep2)
            {
                if (batterystep3)
                {
                    if (batterystep4)
                    {
                        if (batterystep5)
                        {
                            if (batterystep6)
                            {
                                if (batterystep7)
                                {
                                    if (batterystep8)
                                    {
                                        if (batterystep9)
                                        {
                                            if (batterystep10)
                                            {
                                                /*if (batterystep11)
                                                {
                                                    return 10;
                                                }*/
                                                return 9;
                                            }
                                            return 8;
                                        }
                                        return 7;
                                    }
                                    return 6;
                                }
                                return 5;
                            }
                            return 4;
                        }
                        return 3;
                    }
                    return 2;
                }
                return 1;
            }
            return 0;
        }
        return -1;
    }

    GameObject[] getOjectsBat(int x)
    {
        GameObject[] final;
        List<GameObject> ret=new List<GameObject>();
        if (x == 0)
        {
            ret.Add(GameObject.Find("hood"));

        }else if(x == 1)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 2)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 3)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 4)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 5)
        {
            ret.Add(GameObject.Find("car"));

        }else if(x == 6)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 7)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 8)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 9)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }
        final = ret.ToArray();

        return final;
    }

    public GameObject[] getObjectsVideo(int x)
    {
        GameObject[] final;
        List<GameObject> ret = new List<GameObject>();

        if (x == 1)
        {
            ret.Add(GameObject.Find("good tire"));
        }
        else if (x == 2)
        {
            ret.Add(GameObject.Find("Lug Wrench"));
        }
        else if (x == 3)
        {
            ret.Add(GameObject.Find("jack"));
        }
        else if (x == 4)
        {
            ret.Add(GameObject.Find("boosters"));
        }
        else if (x == 5)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
        }
        else if (x == 6)
        {
            ret.Add(GameObject.Find("car manual"));
        }

        final = ret.ToArray();
        return final;
    }


    public void tireButton()
    {
        videoIndex = 1;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }

    }

    public void wrenchButton()
    {
        videoIndex = 2;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }

    }

    public void jackButton()
    {
        videoIndex = 3;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }

    }

    public void boostersButton()
    {
        videoIndex = 4;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }

    }

    public void batteryButton()
    {
        videoIndex = 5;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }

    }

    public void manualButton()
    {
        videoIndex = 6;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }

    }

    public void setNextClip()
    {
        //hard sset to one for now
        videoIndex++;
        int videoClipIndex = videoIndex;
               //x++;
        
        //set which video jud
        
        if (videoClipIndex > videoClips.Length)
        {
            Debug.Log("Going outside video clip array.");
        }
        else
        {
            GameObject[] objects;
            objects = getObjectsVideo(videoClipIndex);
            StartCoroutine(Flashing(objects));

            player.clip = videoClips[videoClipIndex];
            Debug.Log("setting to "+videoClipIndex);
            player.Play();
            Debug.Log("play");

        }
    }

    IEnumerator Flashing(GameObject[] x)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started flashing at timestamp : " + Time.time);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished flashing at timestamp : " + Time.time);
    }

    private void recognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    public void provideHint()
    {
        //used to give hint based of next step
        if (tire)
        {
            int hintindex;
            GameObject[] objects;
    
            Debug.Log("recognized hint");
    
            //set what step
            hintindex = findStepTire();
            //hintindex = findStepBat();
            //set array of objects that need to flash
            objects = getOjectsTire(hintindex);
            //objects = getOjectsBat(hintindex);
    
            if(hintindex>=0 && hintindex < 10)
            {
                test.clip = hintsForTireChange[hintindex];
                //test.clip = hintsForBatteryJump[hintindex];
                test.Play();
        
                StartCoroutine(Flashing(objects));
            }
            else
            {
            Debug.Log("hintindex wack");
            }
        }
        if (battery)
        {
            int hintindex;
            GameObject[] objects;
    
            Debug.Log("recognized hint");
    
            //set what step
            //hintindex = findStepTire();
            hintindex = findStepBat();
            //set array of objects that need to flash
            //objects = getOjectsTire(hintindex);
            objects = getOjectsBat(hintindex);
    
            if(hintindex>=0 && hintindex < 10)
            {
                //test.clip = hintsForTireChange[hintindex];
                test.clip = hintsForBatteryJump[hintindex];
                test.Play();
        
                StartCoroutine(Flashing(objects));
            }
            else
            {
            Debug.Log("hintindex wack");
            }
        }
    }

    public void Check()
    {
        Debug.Log("recognized check");
    }

    public void stepCheck()
    {
        if (tire)
        {
            int step = findStepTire();
            if (step == 1)
            {
                //loosen the lugs a bit
                if ((!GameObject.Find("detail 1").GetComponent<HubCheck>().tight) && (!GameObject.Find("detail 2").GetComponent<HubCheck>().tight) && (!GameObject.Find("detail 3").GetComponent<HubCheck>().tight) && (!GameObject.Find("detail 4").GetComponent<HubCheck>().tight) && (!GameObject.Find("detail 5").GetComponent<HubCheck>().tight))
                {
                    tirestep3 = true;
                    speakTire = true;
                }
            }
            if(step == 2)
            {
                if ((GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation))
                {
                    tirestep4 = true;
                    speakTire = true;
                }
            }
            if(step == 3)
            {
                if ((GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().raised))
                {
                    tirestep5 = true;
                    speakTire = true;
                }
            }
            if(step == 4)
            {
                if ((GameObject.Find("detail 1").GetComponent<HubCheck>().loosened) && (GameObject.Find("detail 2").GetComponent<HubCheck>().loosened) && (GameObject.Find("detail 3").GetComponent<HubCheck>().loosened) && (GameObject.Find("detail 4").GetComponent<HubCheck>().loosened) && (GameObject.Find("detail 5").GetComponent<HubCheck>().loosened))
                {
                    //loosened is off
                    tirestep6 = true;
                    speakTire = true;
                }
            }
            if(step == 5)
            {
                if (GameObject.Find("bad tire Detailed halo").GetComponent<badTireCheck>().off)
                {
                    tirestep7 = true;
                    speakTire = true;
                }
            }
            if(step == 6)
            {
                if (GameObject.Find("goodTireDetailedHalo").GetComponent<goodTireCheck>().on)
                {
                    tirestep8 = true;
                    speakTire = true;
                }
            }
            if(step == 7)
            {
                if ((!GameObject.Find("detail 1").GetComponent<HubCheck>().loosened) && (!GameObject.Find("detail 2").GetComponent<HubCheck>().loosened) && (!GameObject.Find("detail 3").GetComponent<HubCheck>().loosened) && (!GameObject.Find("detail 4").GetComponent<HubCheck>().loosened) && (!GameObject.Find("detail 5").GetComponent<HubCheck>().loosened))
                {
                    //loosened is off
                    tirestep9 = true;
                    speakTire = true;
                }
            }
            if(step == 8)
            {
                if (GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().lowered)
                {
                    tirestep10 = true;
                    speakTire = true;
                }
            }
            if(step == 9)
            {
                if ((GameObject.Find("detail 1").GetComponent<HubCheck>().tight) && (GameObject.Find("detail 2").GetComponent<HubCheck>().tight) && (GameObject.Find("detail 3").GetComponent<HubCheck>().tight) && (GameObject.Find("detail 4").GetComponent<HubCheck>().tight) && (GameObject.Find("detail 5").GetComponent<HubCheck>().tight))
                {
                    Debug.Log("AAAAAAAAAAAAALLLLLLLLLLLLLLLLLL DOOOOOOOOOOOOOOOONE");
                }
            }
            if(step == 10)
            {

            }

        }
    }


    public void setHalos(GameObject[] x)
    {
        for (int i = 0; i < x.Length; i++)
        {
            if (tire)
            {
                int step = findStepTire();
                if (step == 1)
                {
                    if ((x[i] == GameObject.Find("detail 1")) || (x[i] == GameObject.Find("detail 2")) || (x[i] == GameObject.Find("detail 3")) || (x[i] == GameObject.Find("detail 4")) || (x[i] == GameObject.Find("detail 5")))
                    {
                        bool temp = x[i].GetComponent<HubCheck>().tight;
                        if (temp == true)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                    }
                    if ((x[i] == GameObject.Find("wrenchDetailedHalo")))
                    {
                        if(x[i].GetComponent<wrenchCheck>().inLocation){
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 2)
                {
                    if ((x[i] == GameObject.Find("jackDetailedHalo")))
                    {
                        if (x[i].GetComponent<jackCheck>().inLocation)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 3)
                {
                    if ((x[i] == GameObject.Find("jackDetailedHalo")))
                    {
                        if (x[i].GetComponent<jackCheck>().raised)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                    //put somehting that is raising it
                    if(x[i] == GameObject.Find("wrenchDetailedHalo"))
                    {
                        //onjack
                        if (x[i].GetComponent<wrenchCheck>().inLocation)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 4)
                {
                    if ((x[i] == GameObject.Find("detail 1")) || (x[i] == GameObject.Find("detail 2")) || (x[i] == GameObject.Find("detail 3")) || (x[i] == GameObject.Find("detail 4")) || (x[i] == GameObject.Find("detail 5")))
                    {
                        //loosened is off
                        if (x[i].GetComponent<HubCheck>().loosened)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                    if ((x[i] == GameObject.Find("wrenchDetailedHalo")))
                    {
                        //not sure how to do inlocation
                        if(x[i].GetComponent<wrenchCheck>().inLocation){
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 5)
                {
                    if ((x[i] == GameObject.Find("bad tire Detailed halo")))
                    {
                        if (x[i].GetComponent<badTireCheck>().off)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 6)
                {
                    if ((x[i] == GameObject.Find("goodTireDetailedHalo")))
                    {
                        if (x[i].GetComponent<goodTireCheck>().on)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 7)
                {
                    if ((x[i] == GameObject.Find("detail 1")) || (x[i] == GameObject.Find("detail 2")) || (x[i] == GameObject.Find("detail 3")) || (x[i] == GameObject.Find("detail 4")) || (x[i] == GameObject.Find("detail 5")))
                    {
                        //put them back on
                        if (!x[i].GetComponent<HubCheck>().loosened)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                    if ((x[i] == GameObject.Find("wrenchDetailedHalo")))
                    {
                        //not sure how to do inlocation
                        if(x[i].GetComponent<wrenchCheck>().inLocation){
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 8)
                {
                    if ((x[i] == GameObject.Find("jackDetailedHalo")))
                    {
                        if (x[i].GetComponent<jackCheck>().lowered)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 9)
                {
                    if ((x[i] == GameObject.Find("detail 1")) || (x[i] == GameObject.Find("detail 2")) || (x[i] == GameObject.Find("detail 3")) || (x[i] == GameObject.Find("detail 4")) || (x[i] == GameObject.Find("detail 5")))
                    {
                        //put them back on
                        if (!x[i].GetComponent<HubCheck>().tight)
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                    if ((x[i] == GameObject.Find("wrenchDetailedHalo")))
                    {
                        //not sure how to do inlocation
                        if(x[i].GetComponent<wrenchCheck>().inLocation){
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.green;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set green");
                        }
                        else
                        {
                            SerializedObject haloComponent = new SerializedObject(x[i].GetComponent("Halo"));
                            haloComponent.FindProperty("m_Color").colorValue = Color.red;
                            haloComponent.ApplyModifiedProperties();
                            Debug.Log("set red");
                        }
                    }
                }
                if(step == 10)
                {

                }

            }
        }
    }

    //detailed hint stuff
    public void provideHintDetailed()
    {
        //used to give hint based of next step
        if (tire)
        {
            int hintindex;
            GameObject[] objects;
    
            Debug.Log("recognized where");
    
            //set what step
            hintindex = findStepTire();
            //hintindex = findStepBat();
            //set array of objects that need to flash
            objects = getObjectsTireDetailed(hintindex);
            //objects = getOjectsBat(hintindex);
    
            if(hintindex>=0 && hintindex < 10)
            {
                //test.clip = hintsForTireChange[hintindex];
                //test.clip = hintsForBatteryJump[hintindex];
                //test.Play();
                setHalos(objects);
                StartCoroutine(detailedFlashing(objects));
            }
            else
            {
            Debug.Log("hintindex wack");
            }
        }
        if (battery)
        {
            int hintindex;
            GameObject[] objects;
    
            Debug.Log("recognized hint");
    
            //set what step
            //hintindex = findStepTire();
            hintindex = findStepBat();
            //set array of objects that need to flash
            //objects = getOjectsTire(hintindex);
            objects = getOjectsBatDetailed(hintindex);
    
            if(hintindex>=0 && hintindex < 10)
            {
                //test.clip = hintsForTireChange[hintindex];
                //test.clip = hintsForBatteryJump[hintindex];
                //test.Play();
                StartCoroutine(detailedFlashing(objects));
            }
            else
            {
            Debug.Log("hintindex wack");
            }
        }
    }

    IEnumerator detailedFlashing(GameObject[] x)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started flashing at timestamp : " + Time.time);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }
        yield return new WaitForSeconds(1);
        for(int i=0; i<x.Length; i++)
        {
            Behaviour h = (Behaviour)x[i].GetComponent("Halo");
            h.enabled = !h.enabled;
        }

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished flashing at timestamp : " + Time.time);
    }

    GameObject[] getObjectsTireDetailed(int x)
    {
        GameObject[] final;
        List<GameObject> ret=new List<GameObject>();
        if (x == 1)
        {
            ret.Add(GameObject.Find("detail 1"));
            ret.Add(GameObject.Find("detail 2"));
            ret.Add(GameObject.Find("detail 3"));
            ret.Add(GameObject.Find("detail 4"));
            ret.Add(GameObject.Find("detail 5"));
            ret.Add(GameObject.Find("wrenchDetailedHalo"));

        }else if(x == 2)
        {
            ret.Add(GameObject.Find("jackDetailedHalo"));
        }else if(x == 3)
        {
            ret.Add(GameObject.Find("jackDetailedHalo"));
            ret.Add(GameObject.Find("wrenchDetailedHalo"));

        }else if(x == 4)
        {
            ret.Add(GameObject.Find("wrenchDetailedHalo"));
            ret.Add(GameObject.Find("detail 1"));
            ret.Add(GameObject.Find("detail 2"));
            ret.Add(GameObject.Find("detail 3"));
            ret.Add(GameObject.Find("detail 4"));
            ret.Add(GameObject.Find("detail 5"));

        }else if(x == 5)
        {
            ret.Add(GameObject.Find("bad tire Detailed halo"));

        }else if(x == 6)
        {
            ret.Add(GameObject.Find("goodTireDetailedHalo"));

        }else if(x == 7)
        {
            ret.Add(GameObject.Find("detail 1"));
            ret.Add(GameObject.Find("detail 2"));
            ret.Add(GameObject.Find("detail 3"));
            ret.Add(GameObject.Find("detail 4"));
            ret.Add(GameObject.Find("detail 5"));

        }else if(x == 8)
        {
            ret.Add(GameObject.Find("jackDetailedHalo"));

        }else if(x == 9)
        {
            ret.Add(GameObject.Find("detail 1"));
            ret.Add(GameObject.Find("detail 2"));
            ret.Add(GameObject.Find("detail 3"));
            ret.Add(GameObject.Find("detail 4"));
            ret.Add(GameObject.Find("detail 5"));
            ret.Add(GameObject.Find("wrenchDetailedHalo"));

        }else if(x == 10)
        {

        }
        final = ret.ToArray();

        return final;
    }

   GameObject[] getOjectsBatDetailed(int x)
    {
        GameObject[] final;
        List<GameObject> ret=new List<GameObject>();
        if (x == 0)
        {
            ret.Add(GameObject.Find("hood"));

        }else if(x == 1)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 2)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 3)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 4)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 5)
        {
            ret.Add(GameObject.Find("car"));

        }else if(x == 6)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 7)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 8)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }else if(x == 9)
        {
            ret.Add(GameObject.Find("battery/halo effect"));
            ret.Add(GameObject.Find("boosters"));

        }
        final = ret.ToArray();

        return final;
    }




}
