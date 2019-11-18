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

using System;
using System.IO;
using Assimp;
using Assimp.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace UnityMeshImporter
{
    public class SceneController : MonoBehaviour
    {
        private GameObject _root;

        private float _meshScale = 0.001f;
        
        // UI
        private GameObject _loadCanvas;
        private GameObject _errorCanvas;
        private GameObject _errorText;
        private GameObject _scaleInputField;
        private GameObject _scaleSlider;

        public SceneController()
        {
        }

        void Awake()
        {
            // Root object
            _root = GameObject.Find("Root");

            // UI 
            _loadCanvas = GameObject.Find("LoadCanvas");
            _errorCanvas = GameObject.Find("ErrorCanvas");
            _errorText = GameObject.Find("ErrorText");
            _scaleInputField = GameObject.Find("ScaleInputField");
            _scaleSlider = GameObject.Find("ScaleSlider");
            
            var loadButton = GameObject.Find("LoadButton").GetComponent<Button>();
            loadButton.onClick.AddListener(() =>
            {
                _loadCanvas.GetComponent<Canvas>().enabled = false;

                SimpleFileBrowser.FileBrowser.ShowLoadDialog((path) =>
                {
                    try
                    {
                        var ob = MeshImporter.Load(path);
                        ob.transform.SetParent(_root.transform);
                    }
                    catch (Exception e)
                    {
                        _errorCanvas.GetComponent<Canvas>().enabled = true;
                        _errorText.GetComponent<Text>().text = String.Format("Failed to load: {0}\n{1}", path, e.Message);
                        return;
                    }

                    _loadCanvas.GetComponent<Canvas>().enabled = false;
                }, null, false);
            });
            
            var clearButton = GameObject.Find("ClearButton").GetComponent<Button>();
            clearButton.onClick.AddListener(() => { Clear(); });
            
            var okayButton = GameObject.Find("OKButton").GetComponent<Button>();
            okayButton.onClick.AddListener(() => { _errorCanvas.GetComponent<Canvas>().enabled = false; });
        }

        void Start()
        {
            _loadCanvas.GetComponent<Canvas>().enabled = true;
            _errorCanvas.GetComponent<Canvas>().enabled = false;
        }
        
        void Clear()
        {
            // objects
            foreach (Transform objT in _root.transform)
            {
                Destroy(objT.gameObject);
            }
            
            // re-enable panel
            _loadCanvas.GetComponent<Canvas>().enabled = true;
        }

        void OnGUI()
        {
            // UI update
            _scaleInputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text 
                = _scaleSlider.GetComponent<Slider>().value.ToString();
            
        }

        void Update()
        {
            // ESC to quit
            if(Input.GetKey("escape"))
                Application.Quit();
        }
    }
}