using System;
using System.Windows.Forms;

namespace HIKPlayer
{
    internal class WebCommand
    {


        public void Show()
        {
            if (App.Form.InvokeRequired)
            {
                App.Form.Invoke(new MethodInvoker(() =>
                {
                    App.Form.Text = DateTime.Now.ToString();
                }));
            }


        }



        public void Location(int[] args)
        {

            if (App.Form.InvokeRequired)
            {
                App.Form.Invoke(new MethodInvoker(() =>
                {

                    App.Form.Left = args[0];
                    App.Form.Top = args[1];
                    App.Form.Width = args[2];
                    App.Form.Height = args[3];


                }));
            }

        }



        public void Layout1()
        {

            if (App.Form.Wall.InvokeRequired)
            {
                App.Form.Invoke(new MethodInvoker(() =>
                {
                    var panel = App.Form.Wall;

                    for (var i = 0; i < panel.ColumnCount; i++)
                    {
                        if (i == 0)
                        {
                            App.Form.Wall.ColumnStyles[0].Width = 100;

                        }
                        else
                        {
                            App.Form.Wall.ColumnStyles[i].Width = 0;
                        }
                    }

                    for (var i = 0; i < panel.RowCount; i++)
                    {
                        if (i == 0)
                        {
                            App.Form.Wall.RowStyles[0].Height = 100;

                        }
                        else
                        {
                            App.Form.Wall.RowStyles[i].Height = 0;
                        }
                    }

                }));
            }

        }


        public void LayoutAll()
        {

            if (App.Form.Wall.InvokeRequired)
            {
                App.Form.Invoke(new MethodInvoker(() =>
                {
                    var panel = App.Form.Wall;

                    for (var i = 0; i < panel.ColumnCount; i++)
                    {

                        App.Form.Wall.ColumnStyles[i].Width = 100;


                    }

                    for (var i = 0; i < panel.RowCount; i++)
                    {

                        App.Form.Wall.RowStyles[i].Height = 100;

                    }

                }));
            }

        }
    }
}
