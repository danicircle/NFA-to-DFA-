using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project1._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        void InsertInQueue(Queue<string> _queue,string _State,string[,] _DFA)
        {
           // bool visited = false;
            if (_State == "" || _State=="_")
                return;
            for (int i = 0; i < _DFA.GetLength(0); i++)
            {
                if (_DFA[i, 0] == _State)
                    return;
                if (_DFA[i, 0] == null)
                    break;
            }
            if (!_queue.Contains(_State))
                _queue.Enqueue(_State);
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView_NFA.Rows.Add(); dataGridView_NFA.Rows.Add();
            
        }

        void NfaToDfa(string[,] _NFA)
        {
            string[,] DFA = new string[100, _NFA.GetLength(1)];
            Queue<string> ToVisit = new Queue<string>();




            // Copying 1st Row of NFA to DFA 1st Row  and 
            for (int j = 0; j < _NFA.GetLength(1); j++)
            {
                DFA[0, j] = _NFA[0, j].Replace(",", " ").Replace("_"," "); /// Replacing ',' with ' ' 
                InsertInQueue(ToVisit, DFA[0, j], DFA);     // 
            }

            for (int i = 1; i < DFA.GetLength(0); i++)
            {
                
                if (ToVisit.Count != 0)
                {
                    DFA[i, 0] = ToVisit.Dequeue();

                    int index = 0;
                    String[] array = DFA[i, 0].Replace("  ", " ").Split(' ');
                    for (int j = 0; j < DFA[i, 0].Split(' ').Length; j++)
                    {
                        if (DFA[i, 0].Split(' ')[j] != "")
                        {
                            index = GetIndexOf(_NFA, DFA[i, 0].Split(' ')[j]);
                            if (index>=0)
                            {
                                for (int k = 1; k < _NFA.GetLength(1); k++)
                                {

                                    DFA[i, k] += (" " + (ReturnStateElementNotExist(DFA[i, k], (_NFA[index, k].Replace(",", " ")).Replace("_", "")))).Replace("  ", " ");
                                }
                            }
                            else
                            {
                                for (int k = 1; k < _NFA.GetLength(1); k++)
                                {
                                    DFA[i, k] = " ";
                                }

                            }

                           

                        }

                    }
                    for (int k = 1; k < DFA.GetLength(1); k++)
                    {
                        if(DFA[i, k]!=null)
                             InsertInQueue(ToVisit,DFA[i,k].TrimStart(' ').TrimEnd(' ').Replace("  "," "),DFA);
                    }

                }
                else
                    break;
            }

            ArrayToDGV(DFA, dataGridView_DFA);

        }
        string ReturnStateElementNotExist(string _state, string _newState)
        {

            string statee="";
            
            if (_state!=null && _newState !="")
            {
                string[] newState = _newState.Split(' ');
                
                for (int i = 0; i < newState.Length; i++)
                {
                    bool exist=false;
                    for (int j = 0; j < _state.Split(' ').Length; j++)
                    {
                        if (_state.Split(' ')[j] == newState[i])
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                        statee += " "+ newState[i];
                }

                return statee.Replace("  "," ");

            }
           
            return _newState;
        }
        bool CheckStatesExist(string[,] _DFA,string newState)
        {
            newState = newState.Replace("  "," ");
            if (newState == "" || newState == " ")
                return true;

            for (int i = 0; i < _DFA.GetLength(0); i++)
            {
                if (_DFA[i,0]==null)
                    return false;
                
                else if ( _DFA[i,0]==newState)
                    return true;

                
                
            }
            return false;
        }
        int GetIndexOf(string[,] NFA,string value)
        {
            for (int i = 0; i < NFA.GetLength(0); i++)
            {
                if (NFA[i,0]==value)
                {
                    return i;
                }
            }
            return -1;
        }





        private void numericUpDown_NFARows_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_NFARows.Value>dataGridView_NFA.Rows.Count)
            {
                dataGridView_NFA.Rows.Add();
            }
            else
            {
                dataGridView_NFA.Rows.RemoveAt(dataGridView_NFA.Rows.Count - 1);
            }
        }

        private void numericUpDown_NFAColumns_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_NFAColumns.Value > dataGridView_NFA.Columns.Count)
            {
                dataGridView_NFA.Columns.Add("", "");
                dataGridView_DFA.Columns.Add("", "");
            }
            else
            {
                dataGridView_NFA.Columns.RemoveAt(dataGridView_NFA.Columns.Count - 1);
                dataGridView_DFA.Columns.RemoveAt(dataGridView_NFA.Columns.Count - 1);
            }
        }

        private void button_NFAToDFA_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != null)
                {


                    if (FindFinalState(textBox1.Text))
                    {
                        ClearDFA();
                        NFA = GetDataToArray(dataGridView_NFA);
                       
                        
                        if (NFA != null )
                        {
                            NfaToDfa(NFA);
                            for (int l = 0; l < textBox1.Text.Split(',').Length; l++)
                            {
                                MarkFinalAndInitialStatesInDfa(textBox1.Text.Split(',')[l]);
                            }
                            

                        }
                    }
                    else
                        MessageBox.Show("Final State(s) Not Found in given NFA");
                }
                else
                {
                    MessageBox.Show("Final State Must Not Be Null");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
           
        }
        void MarkFinalAndInitialStatesInDfa(string state)
        {
            try
            {
                if (!dataGridView_DFA.Columns.Contains("FS"))
                {
                    dataGridView_DFA.Columns.Add("FS", "Final and Initial");
                    
                }
                dataGridView_DFA["FS", 0].Value = "I";

                for (int i = 0; i < dataGridView_DFA.Rows.Count; i++)
                {
                    if (dataGridView_DFA[0, i].Value != null)
                    {
                        string[] states = dataGridView_DFA[0, i].Value.ToString().Split(' ');
                        if ((!states.Contains("D")) &&states.Contains(state))
                        {
                            dataGridView_DFA["FS", i].Value += ",F";
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private bool FindFinalState(string p)
        {
            int count=0;
            
            string[] finalStates = p.Split(',');

            for (int i = 0; i < dataGridView_NFA.Rows.Count; i++)
            {
                for (int j = 1; j < dataGridView_NFA.Columns.Count; j++)
                {
                    if (dataGridView_NFA[j, i].Value != null && dataGridView_NFA[j, i].Value.ToString() == "D")
                    {
                        MessageBox.Show("D Is reserved for Dead State");
                        return false;

                    }
                    for (int k = 0; k < finalStates.Length; k++)
			        {
                        if (dataGridView_NFA[j, i].Value != null && dataGridView_NFA[j, i].Value.ToString() == finalStates[k])
                        {
                            count++;
                            
                        }
                   
                    }
                   
                }
            }
            if (count >= finalStates.Length)
            {
                return true;
            }
            
            return false;
        }
        void ClearDFA()
        {
            while (dataGridView_DFA.Rows.Count!=1)
            {
                dataGridView_DFA.Rows.RemoveAt(dataGridView_DFA.Rows.Count-2);
            }
        }

        void ArrayToDGV(string[,] _array,DataGridView DGV)
        {
            bool deadState = false;
            bool com = false;
            for (int i = 0; i < _array.GetLength(0) && !com; i++)
            {
                DGV.Rows.Add();
                for (int j = 0; j < _array.GetLength(1) ; j++)
                {
                    if (_array[i, j] != null)
                    {
                        if (_array[i, j] == " ")
                        {
                            _array[i, j] = "D";
                            deadState = true;
                        }
                        DGV[j, i].Value = _array[i, j].TrimStart(' ');

                    }
                    else
                    {
                        com = true;
                        break;
                    }
                }
            }
            if (deadState)
            {
                DGV[0,DGV.Rows.Count-2].Value="D";
                for (int i = 0; i < _array.GetLength(1); i++)
                {
                    DGV[i, DGV.Rows.Count - 2].Value = "D";
                }
            }
        }
        string[,] GetDataToArray(DataGridView DGV)
        {
            try
            {
                string[,] array = new string[DGV.Rows.Count, DGV.Columns.Count];
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    for (int j = 0; j < DGV.Columns.Count; j++)
                    {
                        array[i, j] = DGV[j, i].Value.ToString();
                    }
                }
                return array;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            return null;
        }

        public string[,] NFA { get; set; }

        private void dataGridView_NFA_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel108_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
