using UnityEngine;
using UnityEngine.UI;

public class CrashDriveBootstrap : MonoBehaviour
{
    Rigidbody car;
    WheelCollider[] wheels = new WheelCollider[4];
    Transform[] wheelVisuals = new Transform[4];
    Camera cam;
    Text speed;
    bool touchGas, touchBrake, touchLeft, touchRight;
    Vector3 spawn = new Vector3(0, 1.25f, 0);

    void Start()
    {
        Application.targetFrameRate = 60;
        CreateWorld();
        CreateCar();
        CreateCamera();
        CreateHud();
    }

    void Update()
    {
        if (car == null) return;
        bool gas = touchGas || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool brake = touchBrake || Input.GetKey(KeyCode.Space);
        bool steerLeft = touchLeft || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool steerRight = touchRight || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        throttle = gas; reverse = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        braking = brake; left = steerLeft; right = steerRight;
        if (Input.GetKeyDown(KeyCode.R)) ResetCar();
        if (speed) speed.text = "SPEED  " + Mathf.RoundToInt(car.velocity.magnitude * 3.6f) + " km/h";
        UpdateWheels();
    }
    bool throttle, reverse, braking, left, right;

    void FixedUpdate()
    {
        if (car == null) return;
        float torque = throttle ? 1450f : reverse ? -850f : 0f;
        for (int i=0;i<4;i++) {
            if (wheels[i] == null) continue;
            wheels[i].motorTorque = torque;
            wheels[i].brakeTorque = braking ? 2000f : 0f;
            wheels[i].steerAngle = i < 2 ? ((left ? -1f : 0f) + (right ? 1f : 0f)) * 28f : 0f;
        }
    }

    void CreateWorld()
    {
        RenderSettings.ambientLight = new Color(.55f,.57f,.6f);
        var sun = new GameObject("Sun").AddComponent<Light>();
        sun.type = LightType.Directional; sun.intensity = 1.2f;
        sun.transform.rotation = Quaternion.Euler(48,-35,0);
        Cube("Grass",new Vector3(0,-.5f,0),new Vector3(180,1,180),new Color(.2f,.36f,.19f));
        Cube("Main road",new Vector3(0,.02f,0),new Vector3(12,.12f,100),new Color(.17f,.18f,.2f));
        Cube("Cross road",new Vector3(0,.03f,24),new Vector3(70,.12f,10),new Color(.17f,.18f,.2f));
        for(int z=-45;z<=45;z+=10) Cube("Road marking",new Vector3(0,.1f,z),new Vector3(.18f,.02f,4),new Color(.95f,.88f,.55f));
        var ramp=Cube("Ramp",new Vector3(8,.8f,22),new Vector3(6,1.2f,10),new Color(.45f,.46f,.49f));
        ramp.transform.rotation=Quaternion.Euler(-12,0,0);
        for(int i=0;i<5;i++) {
            var o=Cube("Crash obstacle",new Vector3(-4+i*2,.7f,34),new Vector3(1.2f,1.4f,1.2f),new Color(.75f,.2f,.12f));
            o.AddComponent<Rigidbody>().mass=18;
        }
    }

    void CreateCar()
    {
        var root=new GameObject("Player car"); root.transform.position=spawn;
        car=root.AddComponent<Rigidbody>(); car.mass=1250; car.centerOfMass=new Vector3(0,-.35f,0);
        car.interpolation=RigidbodyInterpolation.Interpolate; car.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
        var body=Cube("Car body",Vector3.zero,new Vector3(1.85f,.55f,3.6f),new Color(.1f,.4f,.92f));
        body.transform.SetParent(root.transform,false); body.transform.localPosition=new Vector3(0,.15f,0);
        var cabin=Cube("Cabin",new Vector3(0,.62f,-.25f),new Vector3(1.35f,.62f,1.55f),new Color(.1f,.17f,.2f));
        cabin.transform.SetParent(root.transform,false); cabin.GetComponent<Collider>().enabled=false;
        Vector3[] p={new(-.98f,-.18f,1.18f),new(.98f,-.18f,1.18f),new(-.98f,-.18f,-1.18f),new(.98f,-.18f,-1.18f)};
        for(int i=0;i<4;i++) {
            var wo=new GameObject("WheelCollider "+i); wo.transform.SetParent(root.transform,false); wo.transform.localPosition=p[i];
            var w=wheels[i]=wo.AddComponent<WheelCollider>(); w.radius=.36f; w.suspensionDistance=.22f; w.mass=24;
            var s=w.suspensionSpring; s.spring=35000; s.damper=4500; w.suspensionSpring=s;
            var mesh=GameObject.CreatePrimitive(PrimitiveType.Cylinder); mesh.name="Wheel visual"; mesh.transform.SetParent(root.transform,false);
            mesh.transform.localPosition=p[i]; mesh.transform.localRotation=Quaternion.Euler(0,0,90); mesh.transform.localScale=new Vector3(.72f,.18f,.72f);
            mesh.GetComponent<Collider>().enabled=false; mesh.GetComponent<Renderer>().material=Material(new Color(.05f,.05f,.06f));
            wheelVisuals[i]=mesh.transform;
        }
    }

    void CreateCamera()
    {
        var g=new GameObject("Follow Camera"); cam=g.AddComponent<Camera>(); g.tag="MainCamera"; cam.fieldOfView=68;
        g.AddComponent<AudioListener>();
    }
    void LateUpdate()
    {
        if(!cam||!car)return;
        Vector3 target=car.position-car.transform.forward*7+Vector3.up*3.5f;
        cam.transform.position=Vector3.Lerp(cam.transform.position,target,Time.deltaTime*5);
        cam.transform.LookAt(car.position+Vector3.up*.6f);
    }
    void CreateHud()
    {
        var c=new GameObject("Mobile HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        c.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
        c.GetComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
        c.GetComponent<CanvasScaler>().referenceResolution=new Vector2(1280,720);
        var es=new GameObject("EventSystem",typeof(UnityEngine.EventSystems.EventSystem),typeof(UnityEngine.EventSystems.StandaloneInputModule));
        speed=MakeText(c.transform,"SPEED 0 km/h",new Vector2(.5f,.92f),new Vector2(360,60),26);
        Button(c.transform,"◀",new Vector2(.09f,.18f),v=>touchLeft=v);
        Button(c.transform,"▶",new Vector2(.23f,.18f),v=>touchRight=v);
        Button(c.transform,"GAS",new Vector2(.85f,.22f),v=>touchGas=v);
        Button(c.transform,"BRAKE",new Vector2(.85f,.09f),v=>touchBrake=v);
        Button(c.transform,"RESET",new Vector2(.92f,.91f),null);
    }
    Text MakeText(Transform parent,string value,Vector2 anchor,Vector2 size,int fontSize)
    {
        var g=new GameObject("Text",typeof(RectTransform),typeof(Text)); g.transform.SetParent(parent,false);
        var r=g.GetComponent<RectTransform>(); r.anchorMin=r.anchorMax=anchor; r.anchoredPosition=Vector2.zero; r.sizeDelta=size;
        var t=g.GetComponent<Text>(); t.text=value; t.font=Resources.GetBuiltinResource<Font>("Arial.ttf"); t.fontSize=fontSize;
        t.alignment=TextAnchor.MiddleCenter; t.color=Color.white; return t;
    }
    void Button(Transform parent,string label,Vector2 anchor,System.Action<bool> setHold)
    {
        var g=new GameObject(label+" Button",typeof(RectTransform),typeof(Image),typeof(UnityEngine.UI.Button),typeof(HoldButton));
        g.transform.SetParent(parent,false); var r=g.GetComponent<RectTransform>(); r.anchorMin=r.anchorMax=anchor; r.anchoredPosition=Vector2.zero; r.sizeDelta=new Vector2(110,90);
        var im=g.GetComponent<Image>(); im.color=new Color(.06f,.08f,.11f,.78f);
        var b=g.GetComponent<UnityEngine.UI.Button>(); b.targetGraphic=im;
        var t=MakeText(g.transform,label,new Vector2(.5f,.5f),new Vector2(100,80),21); t.raycastTarget=false;
        var h=g.GetComponent<HoldButton>(); h.down=()=>{if(setHold!=null)setHold(true);else ResetCar();}; h.up=()=>{if(setHold!=null)setHold(false);};
    }
    void UpdateWheels()
    {
        for(int i=0;i<4;i++) if(wheels[i]&&wheelVisuals[i]) { wheels[i].GetWorldPose(out var p,out var q); wheelVisuals[i].position=p; wheelVisuals[i].rotation=q*Quaternion.Euler(0,0,90); }
    }
    void ResetCar()
    {
        car.position=spawn; car.rotation=Quaternion.identity; car.velocity=Vector3.zero; car.angularVelocity=Vector3.zero;
    }
    GameObject Cube(string n,Vector3 p,Vector3 s,Color c)
    {
        var g=GameObject.CreatePrimitive(PrimitiveType.Cube); g.name=n; g.transform.position=p; g.transform.localScale=s; g.GetComponent<Renderer>().material=Material(c); return g;
    }
    Material Material(Color c)
    {
        Shader sh=Shader.Find("Standard"); if(!sh) sh=Shader.Find("Universal Render Pipeline/Lit");
        var m=new Material(sh); m.color=c; return m;
    }
}
