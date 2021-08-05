//////////////////////////////////////////////////////////////////////////////////
//
//  Architecture of Scent(TM) Software Suite - OVR Unity Framework
//  Copyright (C) 2018-2019 OVR Tech LLC
//
//    OVR Unity Framework is proprietary software: you can modify it under the
//    terms of the Development Kit Agreement signed before receiving the
//    software. This Software is owned by OVR Tech LLC and any modifications,
//    changes, or alterations are subject to the terms of the Agreement.
//
//    OVR Unity Framework is distributed in the hope that it will be useful,for
//    testing purposes, and for feedback, but WITHOUT ANY WARRANTY; without even
//    the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//    See the Agreement for more details.
//
//    You should have received a copy of the Development Kit Agreement before
//    receiving the OVR Unity Framework. If not, this software may be in violation
//    of the Agreement. Please contact info@ovrtechnology.com or click the link
//    below to request the document.
//
//  https://ovrtechnology.com/contact/
//
//////////////////////////////////////////////////////////////////////////////////

using OVR.Data;
using System.Collections;
using UnityEngine;

namespace OVR.Components
{
  /// <summary>
  /// A nested-spherical zone of odor that persists based on distance from a game-object
  /// </summary>
  public class OdorantSphere : Odorant
  {
    public Vector3 Position { get { return transform.TransformPoint(Offset); } }
    public Vector3 Offset;
    [SerializeField] private float _innerRadius = 0.1f;
    [SerializeField] private AnimationCurve _radialScalar = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    [SerializeField] private float _outerRadius = 0.3f;

    public float InnerRadius
    {
      get { return _innerRadius; }
      set { _innerRadius = value; InnerRadiusSqrd = value * value; }
    }
    public float OuterRadius
    {
      get { return _outerRadius; }
      set { _outerRadius = value; OuterRadiusSqrd = value * value; }
    }

    public float OuterRadiusSqrd { get; private set; }
    public float InnerRadiusSqrd { get; private set; }

    /// <summary>
    /// The proximity odorant node registers itself with the Olfactory component.
    /// </summary>
    void Start()
    {
            Validate();
            //create the OdrantCommand
            OdorantCommand = new OdorantCommand(OdorantConfig, OdorantAlgorithm.Burst);

            // Have to do this to set inner and outer radius squared properties
            InnerRadius = _innerRadius;
            OuterRadius = _outerRadius;
    }

    void OnEnable()
    {
      ShouldStop = false;
      StartCoroutine(CoprocessOdorantCommand());
    }

    void OnDisable()
    {
      ShouldStop = true;
    }

    /// <summary>
    /// The proximity odorant node removes itself from the Olfactory's registry.
    /// </summary>
    void OnDestroy()
    {
      ShouldStop = true;
    }

    public OdorantCommand GetOdorantCommand()
    {
      return OdorantCommand;
    }
    //checking to see if we are close to a scent
    public IEnumerator CoprocessOdorantCommand()
    {
        //wait for instance of epithelium and the load balance is not > 10
      yield return new WaitWhile(delegate () { return !OlfactoryEpithelium.Instanced() || OlfactoryEpithelium.WaitForLoadBalance; });
        //keep going if ShouldStop is false
      while (!ShouldStop)
      {
        //increase the odrants processing this frame
        OlfactoryEpithelium.OdorantsProcessingThisFrame++;
        //make sure the component is active or else wait and continue
        if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
        {
          yield return new WaitForSeconds(OlfactoryEpithelium.Get().BurstUpdateInterval);
          continue;
        }
        //if the insatnce is returned
        if (OlfactoryEpithelium.Get())
        {
          //get the distance betweeen the epithelium and the component
          var sqrDistance = Vector3.SqrMagnitude(Position - OlfactoryEpithelium.Get().Position);
          //we should trigger a command, just need to set intensity
          if (sqrDistance < OuterRadiusSqrd)
          {
            //full intensity               
            if (sqrDistance < InnerRadiusSqrd)
            {
              OdorantCommand.Intensity = Intensity;
            }
            //calculate the intensity
            else
            {
              var normalizedIntensity = Mathf.Clamp01(_radialScalar.Evaluate((Mathf.Sqrt(sqrDistance) - InnerRadius) / (OuterRadius - InnerRadius)));
              OdorantCommand.Intensity = (byte)Mathf.Lerp(0.0f, Intensity, normalizedIntensity);
            }
            //add the command
            OlfactoryEpithelium.Get().AddOdorantCommand(OdorantCommand);
          }
        }
        yield return new WaitForSeconds(OlfactoryEpithelium.Get().BurstUpdateInterval);
      }
    }

    void OnDrawGizmosSelected()
    {
      Gizmos.color = OvrColor.inner;
      Gizmos.DrawWireSphere(Position, _innerRadius);
      Gizmos.color = OvrColor.outer;
      Gizmos.DrawWireSphere(Position, _outerRadius);
    }
    private void OnDrawGizmos()
    {
      Gizmos.DrawIcon(transform.position, "Odorants\\OVR_Sphere.png", true);
    }

    private void Validate()
    {
      BaseValidate();

      if (OuterRadius <= 0.0f)
        Debug.LogWarningFormat("<b>[OVR]</b> {0}: The outer radius must be greater than zero.", GetParentList() + gameObject.name);

      if (OuterRadius < InnerRadius)
        Debug.LogWarningFormat("<b>[OVR]</b> {0}: The outer radius ({1}) must be greater than inner radius ({2}).", GetParentList() + gameObject.name, OuterRadius, InnerRadius);

    }
  }
}