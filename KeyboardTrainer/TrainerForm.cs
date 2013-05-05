using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using KeyboardTrainer.Properties;

namespace KeyboardTrainer
{
    public partial class TrainerForm : Form
    {
        public Font Df;
        public StringFormat DrawF;
        public Pen Pen;
        private int _counter;
        private string[] _arr;
        private char _expectedKey;
        private int index;
        private readonly string[] _keyChar;
        private Label[] lblArray;
        public int ButtonsHeight;
        public TrainerForm()
        {
            InitializeComponent();
            ButtonsHeight = 33;
            _keyChar = new[] 
            { 
                "Esc", "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test","Test",
                "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test","Test",
                "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test","Test",
                "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test","Test",
                "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test","Test",
                "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test", "Test","Test",
                "Test","Test",
            };

            var keyTable = new KeyTable(_keyChar, ButtonsHeight);

            foreach (Button key in keyTable)
            {
                pKeyTable.Controls.Add(key);
            }

            lblArray = new Label[55];
            for (var i = 0; i < lblArray.Length; i++)
            {
                lblArray[i] = new Label
                    {
                        AutoSize = true,
                        Location = new Point(i*15, 5),
                        Size = new Size(10, 13),
                        TabIndex = 1000,
                        Text = "-"
                    };
                pBattlefield.Controls.Add(lblArray[i]);
            }
            pBattlefield.CreateGraphics();
            new SolidBrush(Color.Black);
            Df = new Font("Arial", 11);
            DrawF = new StringFormat();
            Pen = new Pen(Color.Black, 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var sourse = new OpenFileDialog { Filter = Resources.resOP };
            if (sourse.ShowDialog() != DialogResult.OK) return;
            string s;
            using (var sr = new StreamReader(sourse.FileName,Encoding.UTF8))
            {
                s = sr.ReadToEnd();
            }
            _arr = s.Split('.');
        }

        public void Show(string sentence)
        {
            for (var i = 0; i < lblArray.Length && i < sentence.Length; i++)
            {
                lblArray[i].Text = sentence[i].ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            string s;
            using (var sr = new StreamReader("test.txt", Encoding.UTF8))
            {
                s = sr.ReadToEnd();
            }
            _arr = s.Split('.');
            index = 0;
            pBattlefield.Refresh();
            Show(_arr[_counter] + ".");

            
            lblArray[index].BorderStyle = BorderStyle.FixedSingle;
            while (!char.IsLetter(_arr[_counter][index]))
            {
                index++;
            }
            _expectedKey = _arr[_counter][index];
        }

        private void WrongKey()
        {
            
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _expectedKey = _arr[_counter][index];
            if (e.KeyChar == _expectedKey)
            {
                pBattlefield.Refresh();
                do
                {
                    index++;
                    if (index < _arr[_counter].Length) continue;
                    _counter++;
                    index = 0;
                    Show(_arr[_counter]);
                    break;
                } while (!char.IsLetter(_arr[_counter][index]) && !char.IsPunctuation(_arr[_counter][index]) &&
                         !char.IsWhiteSpace(_arr[_counter][index]));
                if (index - 1 >= 0)
                {
                    lblArray[index - 1].BorderStyle = BorderStyle.None;
                }
                lblArray[index].BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                WrongKey();
            }
        }

        public class KeyTable : IEnumerable, IEnumerator
        {
            public Padding Padding
            {
                get { return new Padding(1,1);}
                set { throw new InvalidOperationException(); }
            }
            public Margin Margin
            {
                get { return new Margin(3, 3); }
                set { throw new InvalidOperationException(); }
            }
            public readonly int[] KeysCount = new[] { 14,13,13,12,8 };
            public Button[] Key;

            private readonly short _keyIndex;
            private readonly int _offset;
            private readonly Size[] _keySize;

            public static Size[] CalculateKeySize(int h)
            {
                var d = new Size(33, 33);
                var buttonSize = new[]
                {
                    d, d, d, d, d, d, d, d, d, d, d, d, d, new Size((int)(h * 2.03), h), // 1 ряд
                    new Size((int)(h * 1.51), h), d, d, d, d, d, d, d, d, d, d, d, d, // 2 ряд       
                    new Size((int)(h * 1.76), h), d, d, d, d, d, d, d, d, d, d, d, d,  // 3 ряд, Enter
                    new Size((int)(h * 2.24), h), d,d,d,d,d,d,d,d,d,d, new Size((int)(h * 2.85), h),  // 4 ряд
                    new Size((int)(h * 1.48), h), new Size((int)(h * 1.24), h), new Size((int)(h * 1.27), h), new Size((int)(h * 5.91), h), new Size((int)(h * 1.27), h), new Size((int)(h * 1.24), h),new Size((int)(h * 1.24), h), new Size((int)(h * 1.55), h) // 5 ряд
                };
                return buttonSize;
            }

            public KeyTable(IList<string> keyChar, int height)
            {
                Key = new Button[60];
                _keySize = CalculateKeySize(height);

                for (var i = 0; i < 5; i++)
                {
                    _offset = Margin.Left;
                    for (var j = 0; j < KeysCount[i]; j++, _keyIndex++)
                    {
                        Key[_keyIndex] = new Button
                            {
                                Location = new Point(_offset, Margin.Top + (Padding.Y + _keySize[_keyIndex].Height)*i),
                                Size = _keySize[_keyIndex],
                                TabIndex = 8,
                                Text = keyChar[_keyIndex],
                                UseVisualStyleBackColor = true
                            };
                        _offset += _keySize[_keyIndex].Width + Padding.X;
                    }
                }
            }

            int _pos = -1;

            public IEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                _pos++;
                return (_pos < Key.Length);
            }

            public void Reset()
            { _pos = 0; }

            public object Current
            {
                get { return Key[_pos]; }
            }
        }

        public new struct Margin
        {
            public int Left;
            public int Top;
            public Margin(int left, int top)
            {
                Left = left;
                Top = top;
            }
        }

        public new struct Padding
        {
            public int X;
            public int Y;
            public Padding(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
