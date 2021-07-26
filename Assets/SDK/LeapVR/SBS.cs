using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Generic non-distorted Side-by-Side 3D.
    /// </summary>
    [AddComponentMenu("Leap VR/Side-by-Side 3D")]
    public class SBS : Singleton<SBS> {
        public bool EnableInstantly = false;

        public static KeyCode FlipKey = KeyCode.S;
        public static float EyeDistance = .075f;
        public static float EyeRotation = 5;

        /// <summary>
        /// True if 3D cameras are active.
        /// </summary>
        static bool State = false;
        /// <summary>
        /// Pair of the main camera.
        /// </summary>
        static Camera OtherMain;
        /// <summary>
        /// Created secondary cameras.
        /// </summary>
        static readonly List<Camera> OtherEyes = new List<Camera>();
        /// <summary>
        /// Cameras to copy.
        /// </summary>
        static readonly List<Camera> Targets = new List<Camera>();
        /// <summary>
        /// Active SBS handler instance.
        /// </summary>
        static GameObject Holder;

        public static void AddCamera(Camera Target) {
            Targets.Add(Target);
            if (State)
                CreateCopy(Target);
        }

        public static void RemoveCamera(Camera Target) {
            Targets.Remove(Target);
            IEnumerator<Camera> OtherEnum = OtherEyes.GetEnumerator(), TargetEnum = Targets.GetEnumerator();
            while (OtherEnum.MoveNext() && TargetEnum.MoveNext()) {
                Camera TargetCam = TargetEnum.Current;
                if (TargetCam == Target) {
                    Camera OtherEye = OtherEnum.Current;
                    OtherEyes.Remove(OtherEye);
                    Destroy(OtherEye.gameObject);
                    Targets.Remove(TargetCam);
                    return;
                }
            }
        }

        /// <summary>
        /// Copies a component to another object.
        /// </summary>
        static Component CopyComponent(Component Source, GameObject Target) {
            Type ComponentType = Source.GetType();
            Component Copy = Target.AddComponent(ComponentType);
            FieldInfo[] Fields = ComponentType.GetFields();
            int FieldCount = Fields.Length;
            for (int Field = 0; Field < FieldCount; ++Field) {
                FieldInfo Reference = Fields[Field];
                Reference.SetValue(Copy, Reference.GetValue(Source));
            }
            PropertyInfo[] Properties = ComponentType.GetProperties();
            int PropertyCount = Properties.Length;
            for (int Property = 0; Property < PropertyCount; ++Property) {
                PropertyInfo Reference = Properties[Property];
                if (Reference.SetMethod != null && !Reference.Name.Contains("Matrix"))
                    Reference.SetValue(Copy, Reference.GetValue(Source));
            }
            return Copy;
        }

        /// <summary>
        /// Creates a copy of a target camera parented under that camera.
        /// </summary>
        static void CreateCopy(Camera TargetCam) {
            GameObject NewObj = new GameObject();
            NewObj.transform.parent = TargetCam.gameObject.transform;
            NewObj.transform.localPosition = new Vector3(-EyeDistance, 0, 0);
            NewObj.transform.localEulerAngles = new Vector3(0, EyeRotation, 0);
            float FovExtension = Screen.width / (float)Screen.height;
            TargetCam.fieldOfView *= FovExtension;
            TargetCam.orthographicSize *= FovExtension;
            TargetCam.rect = new Rect(.5f, 0, .5f, 1);
            Camera OtherEye = (Camera)CopyComponent(TargetCam, NewObj);
            OtherEye.rect = new Rect(0, 0, .5f, 1);
            // Copy skybox
            Skybox Sky = TargetCam.gameObject.GetComponent<Skybox>();
            if (Sky)
                CopyComponent(Sky, NewObj);
            // Group
            OtherEyes.Add(OtherEye);
            if (TargetCam == Camera.main)
                OtherMain = OtherEye;
        }

        public static bool Enabled {
            get { return State; }
            set {
                if (value == State)
                    return;
                if (Holder)
                    Destroy(Holder);
                if (!value) {
                    IEnumerator<Camera> OtherEnum = OtherEyes.GetEnumerator(), TargetEnum = Targets.GetEnumerator();
                    while (OtherEnum.MoveNext() && TargetEnum.MoveNext()) {
                        Camera OtherEye = OtherEnum.Current, TargetCam = TargetEnum.Current;
                        Destroy(OtherEye.gameObject);
                        TargetCam.rect = new Rect(0, 0, 1, 1);
                        float FovExtension = Screen.height / (float)Screen.width;
                        TargetCam.fieldOfView *= FovExtension;
                        TargetCam.orthographicSize *= FovExtension;
                    }
                    OtherEyes.Clear();
                } else {
                    (Holder = new GameObject()).AddComponent<SBS>();
                    IEnumerator<Camera> TargetEnum = Targets.GetEnumerator();
                    while (TargetEnum.MoveNext())
                        CreateCopy(TargetEnum.Current);
                    DontDestroyOnLoad(Holder);
                }
                State = value;
            }
        }

        void Awake() {
            Targets.Clear();
            AddCamera(Camera.main);
            if (EnableInstantly)
                Enabled = true;
        }

        /// <summary>
        /// Usable height ratio from a side image.
        /// </summary>
        static float WorkingHeight = 1;

        /// <summary>
        /// Top margin for side items to fit inside a 16:9  vertically centered box.
        /// </summary>
        static float TopMargin = 0;

        void Update() {
            // UI precalculations
            float ScreenRatio = Screen.width * .5f / Screen.height;
            WorkingHeight = Mathf.Min(1, ScreenRatio / (16f / 9f));
            TopMargin = (1 - WorkingHeight) * .5f * Screen.height;
            // Re-enable through scenes
            if (OtherEyes.Count != 0 && !OtherEyes[0]) {
                OtherEyes.Clear();
                State = false;
                Enabled = true;
            }
            // Flip eyes by pressing that key
            if (Input.GetKeyDown(FlipKey)) {
                IEnumerator<Camera> OtherEnum = OtherEyes.GetEnumerator(), TargetEnum = Targets.GetEnumerator();
                while (OtherEnum.MoveNext() && TargetEnum.MoveNext()) {
                    Camera OtherEye = OtherEnum.Current, TargetCam = TargetEnum.Current;
                    Rect Temp = TargetCam.rect;
                    TargetCam.rect = OtherEye.rect;
                    OtherEye.rect = Temp;
                }
            }
        }

        public static Ray StereoRay(Vector3 Position) {
            Camera c = Camera.main;
            if (OtherMain) {
                int HalfWidth = Screen.width / 2;
                if ((c.rect.x == 0 && Position.x >= HalfWidth) || (c.rect.x != 0 && Position.x < HalfWidth))
                    c = OtherMain;
            }
            return c.ScreenPointToRay(Position);
        }

        /// <summary>
        /// Size a rectangle to fit in the left eye's 16:9 vertically center box.
        /// </summary>
        public static void PlaceInLeftEye(ref Rect Position) {
            Position.x *= .5f;
            Position.width *= .5f;
            Position.y = Position.y * WorkingHeight + TopMargin;
            Position.height *= WorkingHeight;
        }

        public static void StereoLabel(Rect Position, string Content) {
            if (OtherMain) {
                int OldFontSize = GUI.skin.label.fontSize;
                GUI.skin.label.fontSize = (int)(GUI.skin.label.fontSize * WorkingHeight);
                PlaceInLeftEye(ref Position);
                GUI.Label(Position, Content);
                Position.x += Screen.width * .5f;
                GUI.Label(Position, Content);
                GUI.skin.label.fontSize = OldFontSize;
            } else
                GUI.Label(Position, Content);
        }

        public static void StereoTexture(Rect Position, Texture Texture) {
            if (OtherMain) {
                PlaceInLeftEye(ref Position);
                GUI.DrawTexture(Position, Texture);
                Position.x += Screen.width * .5f;
                GUI.DrawTexture(Position, Texture);
            } else
                GUI.DrawTexture(Position, Texture);
        }
    }
}