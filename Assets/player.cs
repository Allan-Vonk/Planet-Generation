using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private Rigidbody m_rb;
    [SerializeField] private float m_accel;
    private void Start()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
    }
    private void OnCollisionStay(Collision collision)
    {
        m_rb.velocity += m_rb.velocity.normalized * Time.deltaTime * m_accel;
        Debug.Log(m_rb.velocity);
    }
}
