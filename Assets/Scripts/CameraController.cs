/*
 * MIT License
 * 
 * Copyright (c) 2019 Dongho Kang
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMeshImportExample
{
    public class CameraController : MonoBehaviour
    {
        // camera pose control
        [SerializeField]
        private float speed = 0.1f;
        [SerializeField]
        private float sensitivity = 0.5f;
 
        private Camera cam;
        private Vector3 _anchorPoint;
        private Quaternion _anchorRot;
        
        void Update()
        {
            // move by keyboard
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                move += Vector3.forward * speed;
            if (Input.GetKey(KeyCode.S))
                move -= Vector3.forward * speed;
            if (Input.GetKey(KeyCode.D))
                move += Vector3.right * speed;
            if (Input.GetKey(KeyCode.A))
                move -= Vector3.right * speed;
            if (Input.GetKey(KeyCode.E))
                move += Vector3.up * speed;
            if (Input.GetKey(KeyCode.Q))
                move -= Vector3.up * speed;
            transform.Translate(move);
        
            if (!EventSystem.current.IsPointerOverGameObject ()) 
            {
                // only do this if mouse pointer is not on the GUI
                
                // change camera orientation by right drag 
                if (Input.GetMouseButtonDown(1))
                {
                    _anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
                    _anchorRot = transform.rotation;
                }
            
                if (Input.GetMouseButton(1))
                {
                    Quaternion rot = _anchorRot;
                    Vector3 dif = _anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
                    rot.eulerAngles += dif * sensitivity;
                    transform.rotation = rot;
                }
            }
        }
    }
}