using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class Viewpoint : MonoBehaviour
{
    // 1. Declare Variables
    const int widht = 320;
    const int hight = 240;

    const int max_x = widht;
    const int max_y = hight;
    public float moveSpeed;

    Thread receiveThread; //1
    UdpClient client; //2
    int port; //3
    //public GameObject Player; //4
    int dir_code = 0;
    int x = widht / 2;
    int y = hight / 2;
    int z = 0;
    
    float old_norm_x = 0;
    float old_norm_y = 0;
    int counter = 0;
    //bool first = false;
    bool values_used = false;
    float norm_x = 0;
    float norm_y = 0;
    public static float diff_x = 0;
    public static float diff_y = 0;

    // Start is called before the first frame update
    void Start()
    {
        print("********Viewpoint");
        moveSpeed = 2.0f;
        port = 5065; //1 
        InitUDP(); //4
    }

    private void InitUDP()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData)); //1 
        receiveThread.IsBackground = true; //2
        receiveThread.Start(); //3

    }

    private void ReceiveData()
    {
        client = new UdpClient(port); //1
        while (true) //2
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //3
                byte[] data = client.Receive(ref anyIP); //4

                string text = Encoding.UTF8.GetString(data); //5
                //print(">> " + text);
                if (counter == 0)
                {
                    x = Int32.Parse(text);
                    counter = 1;
                    values_used = false;
                    print("x poss " + x);
                }
                else
                {
                    y = Int32.Parse(text);
                    //counter = 2;
                    counter = 0;
                    values_used = true;
                    print("y poss " + y);
                }
                /*
                if (counter == 2)
                {
                    z = Int32.Parse(text);
                    counter = 0;
                    values_used = true;
                }
                */
                norm_x = (2.0f * ((float)x / max_x)) - 1.0f;
                norm_y = (2.0f * ((float)y / max_y)) - 1.0f;

            }
            catch (Exception e)
            {
                print(e.ToString()); //7
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (norm_x == old_norm_x && norm_y == old_norm_y)
        {*/
            diff_x = norm_x - old_norm_x;
            diff_y = norm_y - old_norm_y;

            //print("y coord " + (-1 * moveSpeed * diff_y));
            //transform.Rotate(Input.GetAxis("Mouse Y") * moveSpeed, -Input.GetAxis("Mouse X") * moveSpeed, 0f);
            //print(diff_y);
            //print("x coord " + (-1 * moveSpeed * diff_x));
            //transform.Translate(0.05f,0f, 0f);
            //transform.Translate(-1 * moveSpeed * diff_y, -1 * moveSpeed * diff_x, 0f);
            //destination.x = destination.x + diff_x;
            //transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime* smoothFactor);
            //print("x poss " + (-1 * moveSpeed * diff_x));
            print("diff_y " + diff_y);
            print("diff_x " + diff_x);
            Vector3 direction = new Vector3(-1 * diff_x, -1 * diff_y, 0);
            print("x poss " + direction.x);
            print("y poss " + direction.y);
            StartCoroutine(smooth_move(direction, 5f)); //Calling the coroutine.



            old_norm_y = norm_y;
            old_norm_x = norm_x;
            values_used = true;
        /*}*/
    }



    IEnumerator smooth_move(Vector3 direction, float speed)
    {
        float startime = Time.time;
        Vector3 start_pos = transform.position; //Starting position.
        Vector3 end_pos = transform.position + direction; //Ending position.

        while (start_pos != end_pos && ((Time.time - startime) * speed) < 1f)
        {
            float move = Mathf.Lerp(0, 1, (Time.time - startime) * speed);

            transform.position += direction * move;

            yield return null;
        }
    }

}   
