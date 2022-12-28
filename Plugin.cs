using BepInEx;
using System;
using UnityEngine;
using Utilla;
using System.IO;
using System.Reflection;
using UnityEngine.Audio;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.UI;
using System.ComponentModel;
namespace GorillaAds
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [Description("HauntedModMenu")]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        GameObject ad;
        AudioSource robloxsound;
       public Sprite[] a;
        Image adimg;
        bool closed;
        bool isopen;
        bool justedpressed;
        bool canskip = false;
        bool isadshoweing;
        double Nextadin = 10;
        double Closeadin = 5;
        Text info;
        bool ismodon;
       
        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
            ismodon = true;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
            ismodon = false;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            Array.Resize(ref a, 17);
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("GorillaAds.assets.cool");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            
            GameObject d = bundle.LoadAsset<GameObject>("adui");
            //OfflineVRRig/Actual Gorilla/rig/body/head
           ad = Instantiate(d);
            ad.transform.SetParent(GameObject.Find("Global/Local VRRig/Local Gorilla Player/rig/body/head").transform, false);
            ad.transform.localPosition = new Vector3(0f, 0.1091f, 1.0255f);
            ad.transform.localScale = new Vector3(1.2109f, 1.7182f, 0);
            robloxsound = ad.transform.Find("Audio Source").GetComponent<AudioSource>();
            adimg = ad.transform.Find("e/ad").GetComponent<Image>();
            info = ad.transform.Find("e/info").GetComponent<Text>();
            for (int i = 1; i < 17; i++)
            {
                print(i);
                Sprite ds = bundle.LoadAsset<Sprite>("gtag_ad_" + i);
                    
                 a[i] = ds;
                
               
                print("loaded" + i);
                print(a[i]);

                //ads[i] = ds;
            }
           
            //Console.WriteLine(ads.ToString());
            ad.SetActive(false);

        }

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
            if(ismodon == true)
            {
                List<InputDevice> list = new List<InputDevice>();
                InputDevices.GetDevices(list);

                for (int i = 0; i < list.Count; i++) //Get input
                {
                    if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
                    {
                        list[i].TryGetFeatureValue(CommonUsages.secondaryButton, out closed);
                        list[i].TryGetFeatureValue(CommonUsages.primaryButton, out isopen);
                    }
                }

                if (closed && canskip)
                {

                    ad.SetActive(false);
                    isadshoweing = false;
                    canskip = false;
                }

                if (!isadshoweing)
                {
                    Nextadin -= (double)Time.deltaTime;

                    if (Nextadin <= 0)
                    {
                        Choserandomad();
                        Nextadin = 10;
                    }
                }


                if (isadshoweing && !canskip)
                {
                    Closeadin -= (double)Time.deltaTime;
                    TimeSpan timeSpan = TimeSpan.FromSeconds(Closeadin);
                    info.text = $"YOU CAN CLOSE AD IN: {timeSpan.Seconds}";
                    if (Closeadin <= 0)
                    {
                        canskip = true;
                        Closeadin = 5;
                        info.text = "PRESS X TO CLOSE";
                    }
                }

            } else
            {
                ad.SetActive(false);
                Closeadin = 5;
                Nextadin = 10;
                canskip = false;
                isadshoweing = false;
            }
        }
            

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
        void Choserandomad()
        {
            
            int randomnum = UnityEngine.Random.Range(1, 17);
            Closeadin = UnityEngine.Random.Range(1, 30);
            Nextadin = UnityEngine.Random.Range(1, 20);
            adimg.sprite = a[randomnum];
            ad.SetActive(true);
            robloxsound.Play();
            isadshoweing = true;
           
        }
        void disableMod(bool value)
        {
            if(!value)
            {
                 ad.SetActive(false);
                Closeadin = 5;
                Nextadin = 10;
            }
        }
    }
}
