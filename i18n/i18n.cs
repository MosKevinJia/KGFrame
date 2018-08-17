using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace KGFrame
{
    //language csv example:
    //
    //id,简体中文,繁體中文,English,日本語
    //100,确定,確定,Okey,確定
    //101,取消,取消,Cancel,キャンセル
    //102,开始,開始,Start,から
    //103,显示文本,顯示文本,Display text, テキストを表示する



    public class i18n 
    {

        public static i18n instance;

        /// <summary>
        /// 语言
        /// </summary>
        private Dictionary<string, string> dictLang;

        /// <summary>
        /// 语言列表
        /// </summary>
        private List<string> listLangName;

        /// <summary>
        /// 当前语言编号
        /// </summary>
        private int iCurLanguage = 0;
          
        public i18n()
        {
            i18n.instance = this;  //全局唯一
        }


        /// <summary>
        /// 读取语言文本
        /// </summary>
        public void Load(string res_path)
        {
            ReadCSV(res_path); 
        }

        /// <summary>
        /// 获得语言列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetLanguageList()
        {
            return listLangName;
        } 


        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="id"></param>
        public void ChangeLanguage(int langeindex)
        {
            string name = listLangName[langeindex];
            for (int i = 0; i < CUIControl.Store.Count; i++)
            {
                string key = CUIControl.Store[i].LangID + "_" + name; 
                if (dictLang.ContainsKey(key))
                {
                    CUIControl.Store[i].SetText(dictLang[key]);
                }
            }
            iCurLanguage = langeindex;
        }


        /// <summary>
        /// 设置UI文字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        private void ChangeControlText(CUIControl control)
        {
            string name = listLangName[iCurLanguage];
            string key = control.LangID + "_" + name;
            if (dictLang.ContainsKey(key))
            {
                control.SetText(dictLang[key]);
            }
        }

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        private void ReadCSV(string path)
        {
            //读取csv二进制文件
            TextAsset asset = Resources.Load<TextAsset>(path); 

            if (!asset)
            {
                Debug.LogError("Not Find Language File Error. " + path);
                return;
            }

            string str = Encoding.UTF8.GetString(asset.bytes);   
            string[] line = str.Split("\n"[0]); //行
            string[] title = line[0].Split(","[0]);  //标题

            listLangName = new List<string>();
            dictLang = new Dictionary<string, string>();  

            for (int ti = 1; ti < title.Length; ti++)
            {
                listLangName.Add(title[ti]);  
            } 

            for (int li = 1; li < line.Length - 1; li++)
            {
                string[] s = line[li].Split(","[0]);
                int id = int.Parse(s[0]);
                for (int i = 1; i < s.Length; i++)
                { 
                    dictLang.Add(id + "_" + listLangName[i - 1], s[i]);
                }
            }

        }


    }


}