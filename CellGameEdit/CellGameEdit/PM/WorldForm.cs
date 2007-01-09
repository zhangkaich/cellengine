﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Runtime.Serialization.Formatters;

using javax.microedition.lcdui;

namespace CellGameEdit.PM
{
    [Serializable]
    public partial class WorldForm : Form , ISerializable
    {
        public String id;
        Hashtable UnitList;
        ArrayList WayPoints;
        int CellW = 16;
        int CellH = 16;

        public WorldForm(String name)
        {
            InitializeComponent();
            toolStripTextBox1.Text = CellW.ToString();
            toolStripTextBox2.Text = CellH.ToString();

            id = name;
            
            UnitList = new Hashtable();
            WayPoints = new ArrayList();
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected WorldForm(SerializationInfo info, StreamingContext context)
        {
            InitializeComponent();
            toolStripTextBox1.Text = CellW.ToString();
            toolStripTextBox2.Text = CellH.ToString();

            UnitList = new Hashtable();

            try
            {
                id = (String)info.GetValue("id", typeof(String));
              
                WayPoints = (ArrayList)info.GetValue("WayPoints", typeof(ArrayList));
                
                ArrayList Units = (ArrayList)info.GetValue("Units", typeof(ArrayList));
                for (int i = 0; i < Units.Count;i++ )
                {
                    try
                    {
                        Unit unit = ((Unit)Units[i]);
                        ListViewItem item = new ListViewItem(unit.id);
                        listView1.Items.Add(item);
                        UnitList.Add(item, unit);
                        pictureBox1.Width = Math.Max(unit.x + unit.getWidth(), pictureBox1.Width);
                        pictureBox1.Height = Math.Max(unit.y + unit.getHeight(), pictureBox1.Height);
                        //Console.WriteLine("Level: Load " + unit.id);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(this.id + " : " + err.Message);
                    }
                }

                pictureBox1.Width = (int)info.GetValue("Width", typeof(int));
                pictureBox1.Height = (int)info.GetValue("Height", typeof(int));
            }
            catch (Exception err)
            {
                Console.WriteLine(this.id + " : " + err.Message);
            }
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            testDelUnit();
            try
            {
                info.AddValue("id", id);

                info.AddValue("WayPoints", WayPoints);

                ArrayList Units = new ArrayList();
                for (int i = 0; i < listView1.Items.Count;i++ )
                {
                    try
                    {
                        Units.Add((Unit)UnitList[listView1.Items[i]]);

                        //Console.WriteLine("Level: Add " + ((Unit)UnitList[listView1.Items[i]]).id);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(this.id + " : " + err.Message);
                    }
                }
                info.AddValue("Units", Units );
                info.AddValue("UnitList", UnitList );

                

                info.AddValue("Width",pictureBox1.Width);
                info.AddValue("Height", pictureBox1.Height);

            }catch(Exception err){
                Console.WriteLine(this.id + " : " + err.Message);
            }
        }

        public void Output(System.IO.StringWriter sw)
        {
            testDelUnit();

            // class
            sw.WriteLine();
            sw.WriteLine("class " + this.id + " extends CWorld ");// class
            sw.WriteLine("{");

            // fields
            foreach (ListViewItem item in listView1.Items)
            {
                try
                {
                    Unit unit = ((Unit)UnitList[item]);
                    sw.WriteLine("public C"+unit.type+" "+unit.id+";");
                    sw.WriteLine("private int " + unit.id + "_X = "+unit.x+";");
                    sw.WriteLine("private int " + unit.id + "_Y = "+unit.y+";");
                }
                catch (Exception err)
                {
                    Console.WriteLine(this.id + " : " + err.Message);
                }
            }
            //

            // way points
            sw.WriteLine("public void initPath()");//
            sw.WriteLine("{");//
            sw.WriteLine("    WayPoints = new CWayPoint[" + WayPoints.Count + "];");//
foreach (WayPoint p in WayPoints){try{if (p != null){//
            sw.WriteLine("    WayPoints[" + WayPoints.IndexOf(p) + "] = new CWayPoint(" + (p.rect.X + p.rect.Width / 2) + "," + (p.rect.Y + p.rect.Height / 2) + ");");//
}}catch (Exception err){Console.WriteLine(this.id + " : " + err.Message);}}//
            sw.WriteLine("");//
foreach (WayPoint p in WayPoints) {try{if (p != null){//
foreach (WayPoint l in p.link){try{if (l != null){//
            sw.WriteLine("    WayPoints[" + WayPoints.IndexOf(p) + "].link(WayPoints[" + WayPoints.IndexOf(l) + "]);");//
}}catch (Exception err) { Console.WriteLine(this.id + " : " + err.Message); }}//
}}catch (Exception err) { Console.WriteLine(this.id + " : " + err.Message); }}//
            sw.WriteLine("}");// way point end

            // units
            sw.WriteLine("public void initUnit()");
            sw.WriteLine("{");
            foreach (ListViewItem item in listView1.Items)
            {
                try
                {
                    Unit unit = ((Unit)UnitList[item]);
                    if (unit.type.Equals("Map"))
                    {
                        sw.WriteLine("    addMap("+unit.id+");");
                    }
                    else
                    {
                        sw.WriteLine("    try{");
                        sw.WriteLine("    addSprite(" + unit.id + ");");
                        sw.WriteLine("    " + unit.id + ".X = " + unit.id + "_X;");
                        sw.WriteLine("    " + unit.id + ".Y = " + unit.id + "_Y;");
                        sw.WriteLine("    }catch(Exception err){}");

                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(this.id + " : " + err.Message);
                }
            }
            sw.WriteLine("}");//unit end

            sw.WriteLine("}");//class end

        }

        public void OutputCustom(String script, System.IO.StringWriter output)
        {
            try
            {
                String world = Util.getFullTrunkScript(script, "#<WORLD>", "#<END WORLD>");

                ArrayList maps = new ArrayList();
                ArrayList sprs = new ArrayList();
                foreach (ListViewItem item in listView1.Items)
                {
                    try
                    {
                        Unit unit = ((Unit)UnitList[item]);
                        if(unit.type == "Map") maps.Add(unit);
                        if (unit.type == "Sprite") sprs.Add(unit);
                    }catch (Exception err) { }
                }

                bool fix = false;

                // maps
                do
                {
                    String[] map = new string[maps.Count];
                    for (int i = 0; i < map.Length; i++)
                    {
                        string X = ((Unit)maps[i]).x.ToString();
                        string Y = ((Unit)maps[i]).y.ToString();
                        string ID = ((Unit)maps[i]).id;
                        map[i] = Util.replaceKeywordsScript(world, "#<UNIT MAP>", "#<END UNIT MAP>",
                               new string[] { "<IDENTIFY>", "<INDEX>", "<X>", "<Y>", },
                               new string[] { ID, i.ToString(), X, Y, });
                    }
                    string temp = Util.replaceSubTrunksScript(world, "#<UNIT MAP>", "#<END UNIT MAP>", map);
                    if (temp == null)
                    {
                        fix = false;
                    }
                    else
                    {
                        fix = true;
                        world = temp;
                    }

                } while (fix);

                // sprs
                do
                {
                    String[] spr = new string[sprs.Count];
                    for (int i = 0; i < spr.Length; i++)
                    {
                        string X = ((Unit)sprs[i]).x.ToString();
                        string Y = ((Unit)sprs[i]).y.ToString();
                        string ID = ((Unit)sprs[i]).id;
                        spr[i] = Util.replaceKeywordsScript(world, "#<UNIT SPRITE>", "#<END UNIT SPRITE>",
                               new string[] { "<IDENTIFY>", "<INDEX>", "<X>", "<Y>", },
                               new string[] { ID, i.ToString(), X, Y, });
                    }
                    string temp = Util.replaceSubTrunksScript(world, "#<UNIT SPRITE>", "#<END UNIT SPRITE>", spr);
                    if (temp == null)
                    {
                        fix = false;
                    }
                    else
                    {
                        fix = true;
                        world = temp;
                    }
                } while (fix);

                //way points
                do
                {
                    String[] wp = new string[WayPoints.Count];
                    for (int i = 0; i < wp.Length; i++)
                    {
                        WayPoint p = ((WayPoint)WayPoints[i]);
                        string X = (p.rect.X + p.rect.Width / 2) + "";
                        string Y = (p.rect.Y + p.rect.Height / 2) + "";
                        wp[i] = Util.replaceKeywordsScript(world, "#<WAYPOINT>", "#<END WAYPOINT>",
                               new string[] { "<INDEX>", "<X>", "<Y>", },
                               new string[] { i.ToString(), X, Y, });
                    }
                    string temp = Util.replaceSubTrunksScript(world, "#<WAYPOINT>", "#<END WAYPOINT>", wp);
                    if (temp == null)
                    {
                        fix = false;
                    }
                    else
                    {
                        fix = true;
                        world = temp;
                    }
                } while (fix);

                do
                {
                    ArrayList link = new ArrayList();
                    foreach (WayPoint p in WayPoints)
                    {
                        try
                        {
                            if (p != null)
                            {//
                                foreach (WayPoint l in p.link)
                                {
                                    try
                                    {
                                        if (l != null)
                                        {//
                                            string START = WayPoints.IndexOf(p).ToString();
                                            string END = WayPoints.IndexOf(l).ToString();
                                            link.Add(
                                                Util.replaceKeywordsScript(world, "#<WAYPOINT LINK>", "#<END WAYPOINT LINK>",
                                                   new string[] { "<INDEX>", "<START>", "<END>", },
                                                   new string[] { link.Count.ToString(), START, END, })
                                            );
                                        }
                                    }
                                    catch (Exception err) { Console.WriteLine(this.id + " : " + err.Message); }
                                }//
                            }
                        }
                        catch (Exception err) { Console.WriteLine(this.id + " : " + err.Message); }
                    }//
                    String[] slink = new string[link.Count];
                    slink = (String[])link.ToArray(typeof(String));
                    string temp = Util.replaceSubTrunksScript(world, "#<WAYPOINT LINK>", "#<END WAYPOINT LINK>", slink);
                    if (temp == null)
                    {
                        fix = false;
                    }
                    else
                    {
                        fix = true;
                        world = temp;
                    }
                } while (fix);

                // world key words
                world = Util.replaceKeywordsScript(world, "#<WORLD>", "#<END WORLD>",
                    new string[] { 
                        "<NAME>", 
                        "<UNIT MAP COUNT>" ,
                        "<UNIT SPRITE COUNT>",
                        "<WAYPOINT COUNT>"
                    },
                    new string[] { 
                        this.id, 
                        maps.Count.ToString(),
                        sprs.Count.ToString(),
                        WayPoints.Count.ToString()
                    }
                    );

                output.WriteLine(world);

            }
            catch (Exception err) { Console.WriteLine(this.id + " : " + err.Message); }
        }

        private void WorldForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        private void WorldForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }



        // list view1 map list
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if ((MapForm)e.Data.GetData(typeof(MapForm)) != null)
            {
                MapForm map = ((MapForm)e.Data.GetData(typeof(MapForm)));

                Unit unit = new Unit(map, "Map" + (listView1.Items.Count).ToString("d4")+"_");
                ListViewItem item = new ListViewItem(unit.id);

                listView1.Items.Add(item);
                UnitList.Add(item, unit);

                pictureBox1.Width = Math.Max(unit.x + unit.getWidth(), pictureBox1.Width);
                pictureBox1.Height = Math.Max(unit.y + unit.getHeight(), pictureBox1.Height);

                CellW = map.CellW;
                CellH = map.CellH;

                toolStripTextBox1.Text = CellW.ToString();
                toolStripTextBox2.Text = CellH.ToString();
         
            }
            if ((SpriteForm)e.Data.GetData(typeof(SpriteForm)) != null)
            {
                SpriteForm spr = ((SpriteForm)e.Data.GetData(typeof(SpriteForm)));

                Unit unit = new Unit(spr, "Spr" + (listView1.Items.Count).ToString("d4")+"_");
                ListViewItem item = new ListViewItem(unit.id);

                listView1.Items.Add(item);
                UnitList.Add(item, unit);

            }

            pictureBox1.Refresh();
            //Console.WriteLine("drag");
        }
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
            //Console.WriteLine("enter");
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        
        // picturebox1 dst level
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            javax.microedition.lcdui.Graphics g = new javax.microedition.lcdui.Graphics(e.Graphics);




            toolStripStatusLabel1.Text = "";

            // draw units
            if (toolStripButton7.Checked || toolStripButton8.Checked)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    try
                    {
                        Unit unit = ((Unit)UnitList[item]);

                        if (unit.type == "Map" && toolStripButton7.Checked)
                        {
                            if (toolStripButton10.Checked)
                            {
                                for (int x = 0; x < pictureBox1.Width; x += unit.getWidth())
                                {
                                    for (int y = 0; y < pictureBox1.Height; y += unit.getHeight())
                                    {
                                        Rectangle rect = new Rectangle(
                                            -pictureBox1.Location.X,
                                            -pictureBox1.Location.Y,
                                            panel1.Width,
                                            panel1.Height
                                            );
                                        if (rect.IntersectsWith(
                                             new System.Drawing.Rectangle(
                                                x, y,
                                                unit.getWidth(),
                                                unit.getHeight())
                                                )
                                            )
                                        {
                                        }
                                        {
                                            int tx = unit.x;
                                            int ty = unit.y;
                                            unit.x = x;
                                            unit.y = y;
                                            unit.render(
                                               g,
                                               new System.Drawing.Rectangle(
                                                    -pictureBox1.Location.X-x,
                                                    -pictureBox1.Location.Y-y,
                                                    panel1.Width,
                                                    panel1.Height
                                               ),
                                               listView1.SelectedItems.Contains(item),
                                               toolStripButton1.Checked,
                                               toolStripButton2.Checked
                                            );
                                            unit.x = tx;
                                            unit.y = ty;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                unit.render(
                                   g,
                                   new System.Drawing.Rectangle(
                                        -pictureBox1.Location.X,
                                        -pictureBox1.Location.Y,
                                        panel1.Width,
                                        panel1.Height
                                   ),
                                   listView1.SelectedItems.Contains(item),
                                   toolStripButton1.Checked,
                                   toolStripButton2.Checked
                                );
                            }
                        }

                        if (unit.type == "Sprite" && toolStripButton8.Checked)
                        {
                            unit.render(
                                   g,
                                    new System.Drawing.Rectangle(
                                        -pictureBox1.Location.X,
                                        -pictureBox1.Location.Y,
                                        splitContainer1.Panel2.Width,
                                        splitContainer1.Panel2.Height
                                   ),
                                   listView1.SelectedItems.Contains(item),
                                   toolStripButton1.Checked,
                                   toolStripButton2.Checked
                                );
                        }

                        if (item.Selected)
                        {
                            toolStripStatusLabel1.Text +=
                                "Unit : " +
                                " X=" + unit.x +
                                " Y=" + unit.y
                            ;
                        }
                    }
                    catch (Exception err)
                    {
                    }
                }
            }
            // draw way points
            if (toolStripButton9.Checked)
            {
                foreach (WayPoint p in WayPoints)
                {
                    if (p != null)
                    {
                        p.Render(g);
                        g.setColor(0xff, 0, 0, 0);
                        e.Graphics.DrawString(
                            WayPoints.IndexOf(p).ToString(),
                            this.Font,
                            System.Drawing.Brushes.Black,
                            p.rect.X + p.rect.Width + 1,
                            p.rect.Y - p.rect.Height / 2);

                        if (p.isCheck)
                        {
                            toolStripStatusLabel1.Text +=
                                "Point : " +
                                " X=" + (p.rect.X + p.rect.Width / 2) +
                                " Y=" + (p.rect.Y + p.rect.Height / 2)
                            ;
                        }
                        //g.drawString(WayPoints.IndexOf(p).ToString(), p.rect.X + p.rect.Width + 1, p.rect.Y - p.rect.Height / 2, 0);
                    }
                }

                if (popedWayPoint != null)
                {
                    popedWayPoint.Render(g);
                    g.setColor(0xff, 0xff, 0xff, 0);
                    g.drawLine(
                        popedWayPoint.rect.X + popedWayPoint.rect.Width / 2,
                        popedWayPoint.rect.Y + popedWayPoint.rect.Height / 2 - popedWayPoint.rect.Height,
                        popedWayPoint.rect.X + popedWayPoint.rect.Width / 2,
                        popedWayPoint.rect.Y + popedWayPoint.rect.Height / 2 + popedWayPoint.rect.Height
                        );
                    g.drawLine(
                        popedWayPoint.rect.X + popedWayPoint.rect.Width / 2 - popedWayPoint.rect.Width,
                        popedWayPoint.rect.Y + popedWayPoint.rect.Height / 2,
                        popedWayPoint.rect.X + popedWayPoint.rect.Width / 2 + popedWayPoint.rect.Width,
                        popedWayPoint.rect.Y + popedWayPoint.rect.Height / 2
                        );
                }
                if (selectedWayPoint != null && popedWayPoint != null)
                {
                    g.setColor(0x80, 0xff, 0, 0);
                    g.drawLine(
                         popedWayPoint.rect.X + popedWayPoint.rect.Width / 2,
                         popedWayPoint.rect.Y + popedWayPoint.rect.Height / 2,
                         selectedWayPoint.rect.X + selectedWayPoint.rect.Width / 2,
                         selectedWayPoint.rect.Y + selectedWayPoint.rect.Height / 2
                         );
                }
            }

            if (toolStripButton12.Checked)
            {
                g.setColor(0x80, 0xff, 0xff, 0xff);


                int sx = -pictureBox1.Location.X / CellW;
                int sy = -pictureBox1.Location.Y / CellH;
                int sw = panel1.Width / CellW + 1;
                int sh = panel1.Height / CellH + 1;

                for (int bx = sx; bx < sx + sw; bx++)
                {
                    g.drawLine(
                       bx * CellW,
                       sy * CellH,
                       bx * CellW,
                       sy * CellH + sh * CellH);
                }
                for (int by = sy; by < sy + sh; by++)
                {
                    g.drawLine(
                        sx * CellW,
                        by * CellH,
                        sx * CellW + sw * CellW,
                        by * CellH);
                }
            }
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    if (item.Checked == true)
                    {
                        Unit unit = ((Unit)UnitList[item]);

                        if (unit.type == "Sprite")
                        {
                            if (!toolStripButton11.Checked)
                            {
                                unit.x = e.X;
                                unit.y = e.Y;

                                if (unit.x < 0) unit.x = 0;
                                if (unit.y < 0) unit.y = 0;
                            }
                            else
                            {
                                unit.x = e.X - e.X % CellW + CellW / 2;
                                unit.y = e.Y - e.Y % CellH + CellH / 2;

                                if (unit.x < 0) unit.x = CellW / 2;
                                if (unit.y < 0) unit.y = CellH / 2;
                            }
                           

                            pictureBox1.Width = Math.Max(unit.x + unit.getWidth(), pictureBox1.Width);
                            pictureBox1.Height = Math.Max(unit.y + unit.getHeight(), pictureBox1.Height);
                        }
                    }

                } 
                foreach (WayPoint p in WayPoints)
                {
                    if (p != null && p.isCheck)
                    {

                        if (!toolStripButton11.Checked)
                        {
                            p.rect.X = e.X;
                            p.rect.Y = e.Y;

                            if (p.rect.X < 0) p.rect.X = 0;
                            if (p.rect.Y < 0) p.rect.Y = 0;
                        }
                        else
                        {
                            p.rect.X = e.X - e.X % CellW + CellW / 2 - p.rect.Width/2;
                            p.rect.Y = e.Y - e.Y % CellH + CellH / 2 - p.rect.Height/2;

                            if (p.rect.X < 0) p.rect.X = CellW / 2 - p.rect.Width/2;
                            if (p.rect.Y < 0) p.rect.Y = CellH / 2 - p.rect.Height/2;
                        }
                      

                        pictureBox1.Width = Math.Max(p.rect.X + p.rect.Width, pictureBox1.Width);
                        pictureBox1.Height = Math.Max(p.rect.Y + p.rect.Height, pictureBox1.Height);
                    }
                   
                }
                pictureBox1.Refresh();
            }
            
        }

        int dx = 0;
        int dy = 0;
        WayPoint selectedWayPoint;
        WayPoint popedWayPoint;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            dx = e.X;
            dy = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                for (int i = listView1.Items.Count - 1; i >= 0; i--)
                {
                    listView1.Items[i].Selected = false;
                }

                popedWayPoint = null;
                selectedWayPoint = null;

                foreach (WayPoint p in WayPoints)
                {
                    if (p != null)
                    {
                        p.isCheck = false;
                    }
                }

                bool isChecked = false;

                if (isChecked == false)
                {
                    for (int i = listView1.Items.Count - 1; i >= 0; i--)
                    {
                        Unit unit = ((Unit)UnitList[listView1.Items[i]]);
                        if (new Rectangle(unit.x - unit.w / 2, unit.y - unit.h / 2, unit.w, unit.h).Contains(e.X, e.Y))
                        {
                            isChecked = true;
                            listView1.Items[i].Selected = true;
                            pictureBox1.Refresh();
                            break;
                        }
                    }
                }

                if (isChecked == false)
                {
                    foreach (WayPoint p in WayPoints)
                    {
                        if (p != null)
                        {
                            if (p.rect.Contains(e.X, e.Y))
                            {
                                isChecked = true;
                                p.isCheck = true;
                                selectedWayPoint = p;
                                pictureBox1.Refresh();
                                break;
                            }
                        }
                    }
                }

            }

            if (e.Button == MouseButtons.Right)
            {
                popedWayPoint = null;

                foreach (WayPoint p in WayPoints)
                {
                    if (p != null)
                    {
                        if (p.rect.Contains(e.X, e.Y))
                        {
                            popedWayPoint = p;
                            
                            break;
                        }
                    }
                }

                if (popedWayPoint == null)
                {
                    menuPath.Items[0].Enabled = false;
                    menuPath.Items[1].Enabled = false;
                    menuPath.Items[2].Enabled = true;
                    menuPath.Items[3].Enabled = false;
                }
                else
                {
                    menuPath.Items[0].Enabled = true;
                    menuPath.Items[1].Enabled = true;
                    menuPath.Items[2].Enabled = false;
                    menuPath.Items[3].Enabled = true;
                }

                menuPath.Opacity = 0.5;
                menuPath.Show(pictureBox1,e.Location);

                pictureBox1.Refresh();
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Refresh();
        }

        //show select
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        //show cd
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        //test del disposed
        private void testDelUnit()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                try
                {
                    Unit unit = ((Unit)UnitList[item]);
                    if (unit.isKilled())
                    {
                        listView1.Items.Remove(item);
                        UnitList.Remove(item);

                        pictureBox1.Refresh();
                    }
                }
                catch (Exception err)
                {
                }
            }
        }
        //del
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if (listView1.Items.Contains(item))
                {
                    listView1.Items.Remove(item);
                    UnitList.Remove(item);

                    pictureBox1.Refresh();
                    break;
                }
            }
        }
        //up-back
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.Items.IndexOf(listView1.SelectedItems[0]) > 0)
                {
                    ListViewItem item = listView1.Items[listView1.Items.IndexOf(listView1.SelectedItems[0]) - 1];

                    listView1.Items.Remove(item);

                    listView1.Items.Insert(listView1.Items.IndexOf(listView1.SelectedItems[listView1.SelectedItems.Count - 1]) + 1, item);

                    pictureBox1.Refresh();
                }
            }
        }
        //down-upon
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.Items.IndexOf(listView1.SelectedItems[listView1.SelectedItems.Count-1]) < listView1.Items.Count - 1 )
                {
                    ListViewItem item = listView1.Items[listView1.Items.IndexOf(listView1.SelectedItems[listView1.SelectedItems.Count - 1]) + 1];

                    listView1.Items.Remove(item);

                    listView1.Items.Insert(listView1.Items.IndexOf(listView1.SelectedItems[0]) , item);

                    pictureBox1.Refresh();
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

        }

        //add way point
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
          // menuPath.SourceControl

            WayPoint p = new WayPoint(dx,dy);
            WayPoints.Add(p);

            pictureBox1.Refresh();

        }
        // del way point
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

            if (popedWayPoint == null) return;

            popedWayPoint.DisLinkAll();
            WayPoints.Remove(popedWayPoint);

            pictureBox1.Refresh();
        }
        // link way point
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (selectedWayPoint == null) return;
            if (popedWayPoint == null) return;

            selectedWayPoint.Link(popedWayPoint);

            pictureBox1.Refresh();
        }
        // dislink way point
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (selectedWayPoint == null) return;
            if (popedWayPoint == null) return;

            selectedWayPoint.DisLink(popedWayPoint);

            pictureBox1.Refresh();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        // back color
        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.ShowHelp = true;
            MyDialog.Color = pictureBox1.BackColor;
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = MyDialog.Color;
            }
        }

        private void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {

        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                Unit unit = ((Unit)UnitList[listView1.Items[e.Item]]);
                unit.id = e.Label;

            }catch(Exception err){}
        }

        // center map block
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        // square show
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        //show block line
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            //pictureBox1.Refresh();
        }

        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            if (Cell.Util.stringIsDigit(toolStripTextBox1.Text, 0, toolStripTextBox1.Text.Length) >= toolStripTextBox1.Text.Length &&
                Cell.Util.stringDigitToInt(toolStripTextBox1.Text, 0, toolStripTextBox1.Text.Length) >= 1)
            {
                CellW = Cell.Util.stringDigitToInt(toolStripTextBox1.Text, 0, toolStripTextBox1.Text.Length);
                pictureBox1.Refresh();
            }
            else
            {
                MessageBox.Show("只能输入大于0的数字！");
                toolStripTextBox1.Focus();
            }
        }
        private void toolStripTextBox2_Leave(object sender, EventArgs e)
        {
            if (Cell.Util.stringIsDigit(toolStripTextBox2.Text, 0, toolStripTextBox2.Text.Length) >= toolStripTextBox2.Text.Length &&
              Cell.Util.stringDigitToInt(toolStripTextBox2.Text, 0, toolStripTextBox2.Text.Length) >= 1)
            {
                CellH = Cell.Util.stringDigitToInt(toolStripTextBox2.Text, 0, toolStripTextBox2.Text.Length);
                pictureBox1.Refresh();
            }
            else
            {
                MessageBox.Show("只能输入大于0的数字！");
                toolStripTextBox2.Focus();
            }

        }

       
    }



    [Serializable]
    public partial class WayPoint : ISerializable
    {
        // serial
        public bool isCheck = false;
        public System.Drawing.Rectangle rect;

        public ArrayList link = new ArrayList();

        public WayPoint(int x,int y)
        {
            rect = new Rectangle(x,y,5,5);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected WayPoint(SerializationInfo info, StreamingContext context)
        {
            try
            {
                rect = new Rectangle(
                    (int)info.GetValue("X", typeof(int)), 
                    (int)info.GetValue("Y", typeof(int)),
                    5,5
                    );
                link = (ArrayList)info.GetValue("link", typeof(ArrayList));
            }
            catch (Exception err)
            {
                Console.WriteLine("Path : " + err.Message);
            }
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("X", rect.X);
                info.AddValue("Y", rect.Y);
                info.AddValue("link", link);
            }
            catch(Exception err)
            {
                Console.WriteLine("Path : " + err.Message);
            }
        }

        public bool Link(WayPoint point)
        {
            if (point == null) return false;
            if (point == this) return false;

            if (!this.link.Contains(point))
            {
                this.link.Add(point);
            }
            if (!point.link.Contains(this))
            {
                point.link.Add(this);
            }

            return true;
        }

        public bool DisLink(WayPoint point)
        {
            if (point == null) return false;

            if (this.link.Contains(point))
            {
                this.link.Remove(point);
            }
            if (point.link.Contains(this))
            {
                point.link.Remove(this);
            }

            return true;
        }

        public bool DisLinkAll()
        {
            if (link.Count == 0) return false;

            foreach (WayPoint p in link)
            {
                if (p != null)
                {
                    if (p.link.Contains(this))
                    {
                        p.link.Remove(this);
                    }
                }
            }

            this.link.Clear();

            return true;
        }

        public void Render(javax.microedition.lcdui.Graphics g)
        {
            try
            {
                foreach (WayPoint p in link)
                {
                    if (p != null)
                    {
                        if (isCheck)
                        {
                            g.setColor(0xff, 0, 0xff, 0);
                            g.drawLine(
                                rect.X + 1 + rect.Width / 2,
                                rect.Y + 1 + rect.Width / 2,
                                p.rect.X + 1 + rect.Width / 2,
                                p.rect.Y + 1 + rect.Width / 2);
                            g.drawLine(
                                rect.X - 1 + rect.Width / 2,
                                rect.Y - 1 + rect.Width / 2,
                                p.rect.X - 1 + rect.Width / 2,
                                p.rect.Y - 1 + rect.Width / 2);
                            g.drawLine(
                                rect.X + 1 + rect.Width / 2,
                                rect.Y - 1 + rect.Width / 2,
                                p.rect.X + 1 + rect.Width / 2,
                                p.rect.Y - 1 + rect.Width / 2);
                            g.drawLine(
                                rect.X - 1 + rect.Width / 2,
                                rect.Y + 1 + rect.Width / 2,
                                p.rect.X - 1 + rect.Width / 2,
                                p.rect.Y + 1 + rect.Width / 2);
                        }
                      
                        g.setColor(0x80, 0x80, 0x80, 0xff);
                        g.drawLine(
                                rect.X + rect.Width / 2,
                                rect.Y + rect.Height / 2,
                                p.rect.X + p.rect.Width / 2,
                                p.rect.Y + p.rect.Height / 2);

                        //float degree = Math.Atan();
                        
                    }
                }

                if (isCheck)
                {
                    g.setColor(0x80, 0, 0, 0x80);
                    g.fillRect(rect.X, rect.Y, rect.Width, rect.Height);
                }
                g.setColor(0xff, 0, 0, 0xff);
                g.drawRect(rect.X, rect.Y, rect.Width, rect.Height);
            }
            catch (Exception err)
            {
                Console.WriteLine("Path : " + err.Message);
            }

        }
    
    }

    [Serializable]
    public partial class Unit : ISerializable
    {
        public String id = "null";
        public String type = "null";

        SpriteForm spr = null;
        MapForm map = null;

        public int x=0;
        public int y=0;
        public int w=8;
        public int h=8;
        //public 

        public Unit(SpriteForm spr,String no)
        {
            this.id = no+spr.id;
            this.spr = spr;
            this.w = 8;
            this.h = 8;
            this.type = "Sprite";
        }
        public Unit(MapForm map,String no)
        {
            this.id = no+map.id;
            this.map = map;
            this.w = 8;
            this.h = 8;
            this.type = "Map";
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected Unit(SerializationInfo info, StreamingContext context)
        {
            try
            {
                id = (String)info.GetValue("id", typeof(String));
                x = (int)info.GetValue("x", typeof(int));
                y = (int)info.GetValue("y", typeof(int));
                
                if ((MapForm)info.GetValue("map", typeof(MapForm)) != null)
                {
                    map = (MapForm)info.GetValue("map", typeof(MapForm));
                    this.type = "Map";
                }
                if ((SpriteForm)info.GetValue("spr", typeof(SpriteForm)) != null)
                {
                    spr = (SpriteForm)info.GetValue("spr", typeof(SpriteForm));
                    this.type = "Sprite";
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Unit:" + err.Message);
            }
            

        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("id", id);
                info.AddValue("x", x);
                info.AddValue("y", y);
                info.AddValue("map", map );
                info.AddValue("spr", spr );
            }
            catch(Exception err)
            {
                Console.WriteLine("Unit:" + err.Message);
            }
        }

        public bool isKilled()
        {
            if ( spr == null && map == null ||
                (spr != null && spr.IsDisposed) || 
                (map != null && map.IsDisposed) /*||
                (spr != null && spr.Enabled) || 
                (map != null && map.Enabled)*/  )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int getWidth()
        {
            if (map != null)
                return map.getWidth();
            else
                return w;
        }

        public int getHeight()
        {
            if (map != null)
                return map.getHeight();
            else
                return h;
        }

        public void render(
            javax.microedition.lcdui.Graphics g,
            System.Drawing.Rectangle scope,
            Boolean selected,
            Boolean select,
            Boolean debug)
        {

            if (spr != null )
            {
                spr.Render(g,x,y,debug,0,0);

                if (selected)
                {
                    g.setColor(0xff, 0xff, 0xff, 0xff);
                    g.drawRect(x - w / 2, y - h / 2, w, h);
                    g.setColor(0x80, 0x80, 0x80, 0x80);
                    g.fillRect(x - w / 2, y - h / 2, w, h);
                }
                if (select)
                {
                    g.setColor(0xff, 0xff, 0xff, 0xff);
                    g.drawRect(x - w / 2, y - h / 2, w, h);

                    g.drawLine(x, y - h, x, y + h);
                    g.drawLine(x + w, y, x - w, y);
                }
            }
            else if (map != null)
            {
                map.Render(g, x, y, scope, false, debug, false, 0);
            }

            

           
            

        }   


    }
}