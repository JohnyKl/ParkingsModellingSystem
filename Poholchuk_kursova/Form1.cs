using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poholchuk_kursova
{
    public partial class ParkingWindow : Form
    {
        public ParkingWindow()
        {
            InitializeComponent();

            btnList = new List<Stall>();//ініціалізація списку парковочних місць

            addToArrayAllButtons();//додавання до списку усіх кнопок з форми

            isParkingOpen = true;//ініціалізація прапору "відкриття" автостоянки
        }

        private void addToArrayAllButtons()//функцція, що додає усі кнопки з форми до списку
        {
            foreach (var ctrl in panel1.Controls)//для кожного контрола (елемента) панелі автостоянки на формі
            {
                if (ctrl is Button)//якщо це кнопка
                {
                    btnList.Add(new Stall((Button)ctrl));//додати у список нове парковочне місце, що відповідатиме даній кнопці
                }
            }

            btnList.Reverse();//обернути в зворотньому порядку список, тому що місця біля "в`їзду знаходяться на формі в кінці додавання елементів, а краще, коли перші місця знаходяться першими в списку
        }
        
        private void timer1_Tick(object sender, EventArgs e)//функція, що викликаєтсья обробником переривань клікання таймера
        {
            foreach (Stall stall in btnList)//для кожного парковочного місця
            {
                stall.incrementWaitTime();//збільшити час очікування на секунду
            }
        }

        private void stall_click(object sender, EventArgs e)//функція, що викликається обробниками переривань усіх кнопок, що символізують парковочні місця
        {
            Button clickedButton = (Button)sender;//отримати кнопку, для якої було викликано обробник переривань
            DialogResult result = DialogResult.Cancel;//результат діалогу з "водієм", який "покидає" автостоянку

            foreach (Stall stall in btnList)//для кожного парковочного місця в списку
            {
                if (stall.isEqualButton(clickedButton))//якщо кнопка, для якої викликано обробник переривань рівна кнопці цього парковочного місця
                {
                    float payment = stall.checkPayment();//підрахувати та отримати суму до сплати чи повернення водієві за користування послугами автостоянки

                    stall.pauseTimer(true);//призупинити таймер перебування на парковочному місці

                    if (payment < 0.0f)//якщо оплата від`ємна, вивести повідомлення про необхідність сплати певної суми
                    {
                        result = showMessage(String.Format("Потрібно cплатити {0:##0.00} гривень.", Math.Abs(payment)), "Розрахунок за послуги", MessageBoxButtons.OKCancel);
                    }
                    else if (payment > 0.0f)//якщо оплата додатня, вивести повідомлення про надлишок передоплати та "повернути" решту
                    {
                        result = showMessage("Ваша решта " + Convert.ToString(payment) + " гривень.", "Розрахунок за послуги", MessageBoxButtons.OK);
                    }
                    else//якщо платня рівна 0, тобто передоплата рівна використаним послугам автостоянки, то вивести побажання щасливої дороги
                    {
                        result = showMessage("Щасливої дороги!", "Розрахунок за послуги", MessageBoxButtons.OK);
                    }

                    if (result == DialogResult.OK)//якшо діалог пройшов успішно
                    {
                        stall.freePlace();//звільнити місце

                        isParkingOpen = true;//позначити прапорцем, що є хоча б одне вільне місце на стоянці і її можна відкрити

                        changeCOBoard();//змінити вивіску автостоянки
                    }
                    else stall.pauseTimer(false);//відновити роботу таймера
                }
            }
        }

        private DialogResult showMessage(String message, String caption, MessageBoxButtons buttons)//показати повідомлення користувачу з переданими параметрами
        {
           return MessageBox.Show(message, caption, buttons);
        }

        private void button1_Click(object sender, EventArgs e)//функція, що викликається обробником переривань при натисненні кнопки з тестом "Зайняти місце"
        {
            float payment = 0.0f;//оголошеня та ініціалізація оплати

            payment = (float)Convert.ToDouble(textBox1.Text);//отримання введеної користувачем суми

            bool isStallFind = false;//оголошення та ініціалізація прапорця, що повідомляє знайдене вільне місце чи ні
            int countOfFree = 0;//оголошення та ініціалізація лічильника вільних місць

            for (int i = 0; i < btnList.Count(); i++)//для кожного в списку місць
            {
                Stall stall = btnList.ElementAt(i);//отримати парковочне місце з індексом і в списку 

                if (stall.isFreeStall())//якщо це вільне місце
                {
                    if(!isStallFind)//якщо ще не знайдено вільного місця
                    {
                        stall.takePlace(payment);//зайняти місце

                        isStallFind = true;//встановити прапорець, що місце знайдено
                    }
                    else countOfFree++;  //збільшити лічильник вільних місць
                }              
            }

            if (!isStallFind || countOfFree == 0)//якщо не знайдено вільного місця або лічильник вільних місць рівний 0
            {
                isParkingOpen = false;//"зачинити" автостоянку

                changeCOBoard();//змінити вивіску
            }
        }

        private void changeCOBoard()//функція, що в залежності від значення прапорця наявності вільних місць на стоянці змінює текст та колір вивіски "закрито/відкрито"
        {
            if (isParkingOpen)
            {
                lblCO.Text = "Є ВІЛЬНІ МІСЦЯ";
                lblCO.ForeColor = Color.Chartreuse;
            }
            else
            {
                lblCO.Text = "ЗАЧИНЕНО";
                lblCO.ForeColor = Color.Red;
            }
        }

        private bool isParkingOpen;//прапорець наявності вільних місць на стоянці та її стану "відкрита/закрита"
        private List<Stall> btnList;//список парковочних місць на автостоянці
    }
}
