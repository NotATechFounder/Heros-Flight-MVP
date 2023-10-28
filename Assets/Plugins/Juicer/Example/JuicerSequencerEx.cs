using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JuicerSequencerEx : MonoBehaviour
{
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private Transform[] path;
    [SerializeField] private float duration;

    private void Start()
    {
        DoSequencer();
    }

    public void DoSequencer()
    {
        Transform pathMover = Instantiate(testPrefab, transform.position, Quaternion.identity).transform;

        JuicerSequencer jucierSequencer = new JuicerSequencer();

        foreach (Transform item in path)
        {
            jucierSequencer.Append(pathMover.JuicyLocalMove(item.position, duration));
            jucierSequencer.Append( pathMover.GetComponent<MeshRenderer>().material.JuicyColour(Random.ColorHSV(), duration));
            jucierSequencer.Delay(1f);
        }

        jucierSequencer.AppendCallback(() =>
        {
            Destroy(pathMover.gameObject);
        });

        jucierSequencer.Run();

        jucierSequencer.SetOnCompleted(() =>
        {
            DoSequencer();
            Debug.Log("Completed");
        });
    }
}
