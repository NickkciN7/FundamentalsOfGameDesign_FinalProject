using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtMaterialChange : MonoBehaviour
{
    [SerializeField] Material Common;
    [SerializeField] Material Hurt;

    public void ChangeToHurt(string name)
    {
        if (name.Contains("prince"))
        {
            PrinceChangeToHurt();
        }
        if (name.Contains("Spike"))
        {
            SpikeShellMonsterChangeToHurt();
        }
        if (name.Contains("Humanoid") | name.Contains("Boss"))
        {
            HumanoidMonsterAndBossChangeToHurt();
        }
    }
    public void ChangeToCommon(string name)
    {
        if (name.Contains("prince"))
        {
            PrinceChangeToCommon();
        }
        if (name.Contains("Spike"))
        {
            SpikeShellMonsterChangeToCommon();
        }
        if (name.Contains("Humanoid") | name.Contains("Boss"))
        {
            HumanoidMonsterAndBossChangeToCommon();
        }
    }


    void PrinceChangeToHurt()
    {
        transform.Find("Prince").GetComponent<SkinnedMeshRenderer>().material = Hurt;
        transform.Find("Armature").Find("Root").Find("Spine1").Find("Spine2").Find("Head").Find("HairCopy").GetComponent<MeshRenderer>().material = Hurt;
    }
    void PrinceChangeToCommon()
    {
        transform.Find("Prince").GetComponent<SkinnedMeshRenderer>().material = Common;
        transform.Find("Armature").Find("Root").Find("Spine1").Find("Spine2").Find("Head").Find("HairCopy").GetComponent<MeshRenderer>().material = Common;
    }


    void SpikeShellMonsterChangeToHurt()
    {
        transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material = Hurt;
        transform.Find("Shell").GetComponent<MeshRenderer>().material = Hurt;
    }
    void SpikeShellMonsterChangeToCommon()
    {
        transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material = Common;
        transform.Find("Shell").GetComponent<MeshRenderer>().material = Common;
    }


    void HumanoidMonsterAndBossChangeToHurt()
    {
        transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material = Hurt;
        transform.Find("Armature").Find("Root").Find("Spine1").Find("Spine2").Find("Head").Find("Helmet").GetComponent<MeshRenderer>().material = Hurt;
    }
    void HumanoidMonsterAndBossChangeToCommon()
    {
        transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material = Common;
        transform.Find("Armature").Find("Root").Find("Spine1").Find("Spine2").Find("Head").Find("Helmet").GetComponent<MeshRenderer>().material = Common;
    }

    
}
