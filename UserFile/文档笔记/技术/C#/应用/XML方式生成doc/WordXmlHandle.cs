using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public class WordXmlHandle
{
    public static XmlDocument LoadXmlFile(string ppFilePath)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(ppFilePath);

        return xmlDoc;
    }

    public static XmlNamespaceManager GetWordXmlNamespaceManager(XmlDocument ppXmlDoc)
    {
        System.Xml.XmlNamespaceManager xnm = new System.Xml.XmlNamespaceManager(ppXmlDoc.NameTable);
        xnm.AddNamespace("w", "http://schemas.microsoft.com/office/word/2003/wordml");
        xnm.AddNamespace("v", "urn:schemas-microsoft-com:vml");
        xnm.AddNamespace("aml", "http://schemas.microsoft.com/aml/2001/core");
        xnm.AddNamespace("wx", "http://schemas.microsoft.com/office/word/2003/auxHint");

        return xnm;
    }

    public static void SetBookmarkText(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, string ppBookmarkName, string ppText)
    {
        System.Xml.XmlNodeList nodeList = ppXmlDoc.SelectNodes("//aml:annotation[@w:type='Word.Bookmark.Start'][@w:name='" + ppBookmarkName + "']", ppNamespaceManager);
        foreach (XmlNode node in nodeList)
        {
            SetBookmarkText(ppXmlDoc, ppNamespaceManager, node, ppText);
        }
    }

    public static void SetBookmarkText(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, XmlNode ppNode, string ppText)
    {
        ///
        /// 找到bookmark结束标记
        /// 
        if (ppNode.NextSibling.Name.ToString() == "aml:annotation")
        {
            ///
            /// 得到ppNode上一个兄弟节点style ("w:rPr")的克隆，以备添给新加内容，样式继承
            ///
            XmlNode mmParentNode = ppNode.ParentNode;

            XmlNode mmNodeStyle = null;
            if (ppNode.PreviousSibling != null)
            {
                mmNodeStyle = ppNode.PreviousSibling.CloneNode(true);
            }
            else
            {
                mmNodeStyle = mmParentNode.SelectNodes("//w:r", ppNamespaceManager)[0].CloneNode(true);
            }

            XmlElement mmNewR = ppXmlDoc.CreateElement("w:r", ppNamespaceManager.LookupNamespace("w"));
            ppNode.AppendChild(mmNewR);

            if (mmNodeStyle.SelectSingleNode("//w:rPr", ppNamespaceManager) != null)
            {
                mmNewR.AppendChild(mmNodeStyle.SelectSingleNode("//w:rPr", ppNamespaceManager));
            }

            XmlElement mmNewT = ppXmlDoc.CreateElement("w:t", ppNamespaceManager.LookupNamespace("w"));
            mmNewR.AppendChild(mmNewT);

            XmlNode mmNewNode = ppXmlDoc.CreateNode(XmlNodeType.Text, "w:t", ppNamespaceManager.LookupNamespace("w"));
            mmNewNode.Value = ppText; //value

            mmNewT.AppendChild(mmNewNode);
        }
        else
        {
            System.Xml.XmlNode mmNextNode = ppNode.NextSibling;

            System.Xml.XmlNode node = mmNextNode.SelectSingleNode("descendant::w:t", ppNamespaceManager);
            while (node == null)
            {
                if (mmNextNode.NextSibling == null)
                {
                    break;
                }

                mmNextNode = mmNextNode.NextSibling;
                node = mmNextNode.SelectSingleNode("descendant::w:t", ppNamespaceManager);
            }

            if (node != null)
            {
                node.InnerText = ppText;
            }
        }
        //else if (ppNode.NextSibling.Name.ToString() == "w:tc")
        //{
        //    System.Xml.XmlNode node = ppNode.NextSibling.SelectSingleNode("./w:p/w:r/w:t", ppNamespaceManager);
        //    if (node != null)
        //    {
        //        node.InnerText = ppText;
        //    }
        //}
        //else if (ppNode.NextSibling.Name.ToString() == "w:r")
        //{
        //    System.Xml.XmlNode node = ppNode.NextSibling.SelectSingleNode("./w:t", ppNamespaceManager);
        //    if (node != null)
        //    {
        //        node.InnerText = ppText;
        //    }
        //}
    }

    #region 多行文本打印
    public static void SetBookmarkTextWithMultiLine(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, string ppBookmarkName, string ppText)
    {
        System.Xml.XmlNodeList nodeList = ppXmlDoc.SelectNodes("//aml:annotation[@w:type='Word.Bookmark.Start'][@w:name='" + ppBookmarkName + "']", ppNamespaceManager);
        foreach (XmlNode node in nodeList)
        {
            SetBookmarkTextWithMultiLine(ppXmlDoc, ppNamespaceManager, node, ppText);
        }
    }

    public static void SetBookmarkTextWithMultiLine(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, XmlNode ppNode, string ppText)
    {
        ///
        /// 找到bookmark结束标记
        /// 
        if (ppNode.NextSibling.Name.ToString() == "aml:annotation")
        {
            ///
            /// 得到ppNode上一个兄弟节点style ("w:rPr")的克隆，以备添给新加内容，样式继承
            ///
            XmlNode mmParentNode = ppNode.ParentNode;

            XmlNode mmNodeStyle = null;
            if (ppNode.PreviousSibling != null)
            {
                mmNodeStyle = ppNode.PreviousSibling.CloneNode(true);
            }
            else
            {
                mmNodeStyle = mmParentNode.SelectNodes("//w:r", ppNamespaceManager)[0].CloneNode(true);
            }

            XmlElement mmNewR = ppXmlDoc.CreateElement("w:r", ppNamespaceManager.LookupNamespace("w"));
            ppNode.AppendChild(mmNewR);

            if (mmNodeStyle.SelectSingleNode("//w:rPr", ppNamespaceManager) != null)
            {
                mmNewR.AppendChild(mmNodeStyle.SelectSingleNode("//w:rPr", ppNamespaceManager));
            }

            string[] mmTexts = ppText.Split('\n');
            for (int i = 0; i < mmTexts.Length; i++)
            {
                if (i > 0)
                {
                    XmlElement mmNewBR = ppXmlDoc.CreateElement("w:br", ppNamespaceManager.LookupNamespace("w"));
                    mmNewR.AppendChild(mmNewBR);
                }

                XmlElement mmNewT = ppXmlDoc.CreateElement("w:t", ppNamespaceManager.LookupNamespace("w"));
                mmNewR.AppendChild(mmNewT);
                XmlNode mmNewNode = ppXmlDoc.CreateNode(XmlNodeType.Text, "w:t", ppNamespaceManager.LookupNamespace("w"));
                mmNewNode.Value = mmTexts[i];
                mmNewT.AppendChild(mmNewNode);
            }
        }
        else
        {
            System.Xml.XmlNode mmNextNode = ppNode.NextSibling;

            System.Xml.XmlNode node = mmNextNode.SelectSingleNode("descendant::w:t", ppNamespaceManager);
            while (node == null)
            {
                if (mmNextNode.NextSibling == null)
                {
                    break;
                }

                mmNextNode = mmNextNode.NextSibling;
                node = mmNextNode.SelectSingleNode("descendant::w:t", ppNamespaceManager);
            }

            if (node != null)
            {
                node.InnerXml = ppText.Replace("\n","<w:br />");
            }
        }
    }
    #endregion

    #region 照片打印(单位：毫米)
    public static void SetBookmarkPhoto(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, string ppBookmarkName, string ppPicUrl,int ppWidth,int ppHeight)
    {
        System.Xml.XmlNodeList nodeList = ppXmlDoc.SelectNodes("//aml:annotation[@w:type='Word.Bookmark.Start'][@w:name='" + ppBookmarkName + "']", ppNamespaceManager);
        foreach (XmlNode node in nodeList)
        {
            SetBookmarkPhoto(ppXmlDoc, ppNamespaceManager, node, ppPicUrl,ppWidth,ppHeight);
        }
    }

    public static void SetBookmarkPhoto(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, XmlNode ppNode, string ppPicUrl, int ppWidth, int ppHeight)
    {
        string mmID = Guid.NewGuid().ToString();
        System.IO.FileInfo mmPhoto = new System.IO.FileInfo(ppPicUrl);
        string mmXml = "<w:pict>";
        mmXml += "<v:shapetype id=\"_x0000_t75\" coordsize=\"21600,21600\" o:spt=\"75\" o:preferrelative=\"t\" path=\"m@4@5l@4@11@9@11@9@5xe\" filled=\"f\" stroked=\"f\">";
        mmXml += "<v:stroke joinstyle=\"miter\"/>";
        mmXml += "<v:path o:extrusionok=\"f\" gradientshapeok=\"t\" o:connecttype=\"rect\"/>";
        mmXml += "<o:lock v:ext=\"edit\" aspectratio=\"t\"/>";
        mmXml += "</v:shapetype>";
        mmXml += "<w:binData w:name=\"wordml://" + mmID + mmPhoto.Extension+ "\" xml:space=\"preserve\">";
        mmXml += ImgToBase64String(ppPicUrl);//base64
        mmXml += "</w:binData>";
        mmXml += "<v:shape id=\"图片 0\" o:spid=\"_x0000_i1025\" type=\"#_x0000_t75\" alt=\"u=104380538,1272259492&amp;fm=21&amp;gp=0"+mmPhoto.Extension+"\" style=\"width:"+ppWidth+"mm;height:"+ppHeight+"mm;visibility:visible;mso-wrap-style:square\">";
        mmXml += "<v:imagedata src=\"wordml://" + mmID + mmPhoto.Extension+ "\" o:title=\"u=104380538,1272259492&amp;fm=21&amp;gp=0\"/>";
        mmXml += "</v:shape>";
        mmXml += "</w:pict>";
        ppNode.InnerXml = mmXml;
    }

    //图片转为ase64编码的文本
    private static string ImgToBase64String(string ppPhotoPath)
    {
        try
        {
            System.Drawing.Image mmPhoto = System.Drawing.Image.FromFile(ppPhotoPath);
            System.Drawing.Imaging.ImageFormat format = mmPhoto.RawFormat;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                if (format.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                {
                    mmPhoto.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Png))
                {
                    mmPhoto.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                {
                    mmPhoto.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                {
                    mmPhoto.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                }
                else if (format.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                {
                    mmPhoto.Save(ms, System.Drawing.Imaging.ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);

                string mmReturn = Convert.ToBase64String(buffer);
                return mmReturn;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("转换失败\nException:" + ex.Message);
        }
    }
    #endregion
    //public static XmlNode GetTableNodeOfBookmark(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, string ppBookmarkName)
    //{
    //    System.Xml.XmlNodeList nodeList = ppXmlDoc.SelectNodes("//aml:annotation[@w:type='Word.Bookmark.Start'][@w:name='" + ppBookmarkName + "']", ppNamespaceManager);

    //    XmlNode mmBookmarkNode = null;
    //    if (nodeList.Count > 0)
    //    {
    //        mmBookmarkNode = nodeList[0];
    //    }
    //    else
    //    {
    //        return null;
    //    }

    //    XmlNode mmTableNode = mmBookmarkNode.ParentNode;
    //    while (mmTableNode.Name.ToString() != "w:tbl")
    //    {
    //        mmTableNode = mmTableNode.ParentNode;
    //    }

    //    return mmTableNode;
    //}


    //public static XmlNode GetFristTRNodeOfBookmark(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, string ppBookmarkName)
    //{
    //    XmlNode mmTableNode = GetTableNodeOfBookmark(ppXmlDoc, ppNamespaceManager, ppBookmarkName);
    //    if (mmTableNode == null)
    //    {
    //        return null;
    //    }

    //    return mmTableNode.SelectSingleNode("w:tr", ppNamespaceManager);
    //}


    public static XmlNode GetTRNodeOfBookmark(XmlDocument ppXmlDoc, XmlNamespaceManager ppNamespaceManager, string ppBookmarkName)
    {
        System.Xml.XmlNodeList nodeList = ppXmlDoc.SelectNodes("//aml:annotation[@w:type='Word.Bookmark.Start'][@w:name='" + ppBookmarkName + "']", ppNamespaceManager);

        XmlNode mmBookmarkNode = null;
        if (nodeList.Count > 0)
        {
            mmBookmarkNode = nodeList[0];
        }
        else
        {
            return null;
        }

        XmlNode mmTRNode = mmBookmarkNode.ParentNode;
        while (mmTRNode.Name.ToString() != "w:tr")
        {
            mmTRNode = mmTRNode.ParentNode;
        }

        return mmTRNode;
    }
}
