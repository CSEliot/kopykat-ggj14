using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class UILogic : MonoBehaviour, IEventListener
    {

        public Texture2D Crosshair;
        public Texture2D Healthbar;
        public Texture2D TargetMarker;
        public Texture2D MissileTargetMarker;
        public GUIText MissileAmmoCount;
        public GUIText BulletAmmoCount;
        public GUIText AIStatusLabel;
        public GUIText KillCount;
        //programmer art for now!
        private Texture2D healthbarFill;
        private Texture2D healthbarHealthFill;

        private PlayerLogic playerLogic;
        //private ActorController actorCtrl;

        private Camera actorCamera;
        private int numKills = 0;

        //vars for UI proportions - all proportions in range 0.0-1.0
        private float healthbarWidth;
        private float healthbarHeight;
        private float aspectRatio;
        //private GameObject markerObj;
        //private GUITexture lockOnMarker;

        // Use this for initialization
        void Start()
        {
            aspectRatio = Screen.width / Screen.height;
            playerLogic = GetComponent<PlayerLogic>();
            //actorCtrl = playerLogic.ActorCtrl;
            //if(playerLogic.ActorCtrl != null)
            //{
            //}
            //the healthbar fill textures will be stretched to fill a rect, so they only need to be 1x1 textures
            healthbarFill = new Texture2D(1, 1, TextureFormat.RGB24, false);
            healthbarFill.SetPixel(0, 0, Color.red);
            healthbarFill.Apply();
            healthbarHealthFill = new Texture2D(1, 1, TextureFormat.RGB24, false);
            healthbarHealthFill.SetPixel(0, 0, Color.green);
            healthbarHealthFill.Apply();

            //we'd like to log player kills
            EventManager.AddListener(EventType.ActorKilled, this);
        }

        // Update is called once per frame
        void Update()
        {
            //update any 3D elements

        }

        //Note that this relies deeply on there being a PlayerLogic system linked up;
        //It'll crap itself if there isn't and the game'll seize up like crazy.
        void OnGUI()
        {
            actorCamera = playerLogic.CameraRig.GetComponentInChildren<Camera>();
        }

        void DrawHealthBar(float healthRatio)
        {
            float clampedRatio = Mathf.Max(healthRatio, 0.0f);
            //draw the back, for now it'll be red
            //health bar's centered at the screen, 85% up
            //note to self, change this to things I can actually read
            GUI.DrawTexture(MakeVPSpaceRect((0.5f - (0.33f / 2.0f)), (0.9f + (0.125f / 2.0f)), 0.33f * Screen.width, 0.01f * Screen.height),
                                    healthbarFill,
                                    ScaleMode.StretchToFill);
            //then draw the actual health
            GUI.DrawTexture(MakeVPSpaceRect((0.5f - (0.33f / 2.0f)), (0.9f + (0.125f / 2.0f)), clampedRatio * 0.33f * Screen.width, 0.01f * Screen.height),
                                    healthbarHealthFill,
                                    ScaleMode.StretchToFill);
        }

        void DrawAmmoBar(float ammoRatio)
        {
            float clampedRatio = Mathf.Max(ammoRatio, 0.0f);
            //then draw the actual health
            GUI.DrawTexture(MakeVPSpaceRect((0.5f - (0.33f / 2.0f)), (0.1f - (0.125f / 2.0f)), clampedRatio * 0.33f * Screen.width, 0.01f * Screen.height),
                                    healthbarHealthFill,
                                    ScaleMode.StretchToFill);
        }

        //vpPosition is in viewport coordinates
        void DrawEnemyHealthBar(float enemyHealthRatio, Vector3 vpPosition, float depthFactor)
        {
            //pretty much same as DrawHealthBar, but healthbar's scaled
            float adjustedDepthFactor = depthFactor / 8;
            float maxWidthRatio = 0.125f;
            float widthRatio = Mathf.Min(maxWidthRatio * adjustedDepthFactor, maxWidthRatio);
            float clampedRatio = Mathf.Max(enemyHealthRatio, 0.0f);
            float fullBarWidth = widthRatio * Screen.width;
            float barHeight = adjustedDepthFactor * 0.025f * Screen.height;
            Vector3 drawPos = vpPosition;
            drawPos.x = drawPos.x - 0.00028f * widthRatio * Screen.width;
            drawPos.y = drawPos.y + 0.0075f * barHeight;
            GUI.DrawTexture(MakeVPSpaceRect(drawPos.x, drawPos.y, fullBarWidth, barHeight),
                                    healthbarFill,
                                    ScaleMode.StretchToFill);
            GUI.DrawTexture(MakeVPSpaceRect(drawPos.x, drawPos.y, enemyHealthRatio * fullBarWidth, barHeight),
                                    healthbarHealthFill,
                                    ScaleMode.StretchToFill);
        }

        #region Rectangle Management
        private Rect MakeVPSpaceRect(float left, float top, float width, float height)
        {
            return new Rect(left * Screen.width,
                            (1.0f - top) * Screen.height,
                            width,
                            height);
        }

        private Rect MakeCenteredVPSpaceRect(float left, float top, float width, float height)
        {
            return new Rect((left / 2.0f - width / 2.0f) * Screen.width,
                            (top / 2.0f - height / 2.0f) * Screen.height,
                            width,
                            height);
        }

        private void DrawVPSpaceRect(float left, float top, Texture2D tex, float scale)
        {
            GUI.DrawTexture(MakeVPSpaceRect(left, top, tex.width * scale, tex.height * scale), tex);
        }

        private void DrawVPSpaceRect(float left, float top, Texture2D tex)
        {
            DrawVPSpaceRect(left, top, tex, 1.0f);
        }

        private void DrawCenteredVPSpaceRect(float x, float y, Texture2D tex, float scale)
        {
            int texWidth = (int)(tex.width * scale);
            int texHeight = (int)(tex.height * scale);
            int xPos = (int)(x * Screen.width);
            int yPos = (int)((1.0f - y) * Screen.height); //screen coordinates have inverted y axis!
            Rect texRect = new Rect(xPos - 0.5f * texWidth, yPos - 0.5f * texWidth, texWidth, texHeight);
            GUI.DrawTexture(texRect, tex);
        }

        private void DrawCenteredVPSpaceRect(float x, float y, Texture2D tex)
        {
            DrawCenteredVPSpaceRect(x, y, tex, 1.0f);
        }
        #endregion

        public bool OnEvent(IEvent eventInstance)
        {
            switch (eventInstance.Type)
            {
                case EventType.ActorKilled:
                    ActorKilledEvent castEvent = (ActorKilledEvent)eventInstance;
                    if (castEvent.Killer == playerLogic.Actor.gameObject)
                    {
                        numKills++;
                    }
                    //don't have to constantly update this now!

                    return true;
                default:
                    return false;
            }
        }
    }
}