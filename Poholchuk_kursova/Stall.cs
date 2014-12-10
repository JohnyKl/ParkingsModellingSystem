using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poholchuk_kursova
{
    class Stall
    {
        private Button btnStall;//кнопка на формі, що відповідає парковочному місцю
        private bool   isFree;//прапорець, що позначає чи вільне місце
        private bool   isPaused;//прапорець, за значенням якого призупиняється таймер у момент, коли водій виїзжає з парковочного місця та їде до виїзду розраховуватись
        private long   waitTime;//час(в секундах), впродовж якого місце було зайнято
        private long   paidTime;//час (в секундах), за який відраховано кошти з внесеної оплати
        private int    number;//код місця
        private float  payment;//внесена поточна оплата за місце

        public Stall(Button btn)//конструктор класу
        {
            //ініціалізація компонентів
            btnStall = btn;
            isFree   = true;
            isPaused = false;
            waitTime = 0L;
            paidTime = 0L;
            payment  = 0.0f;
            number   = Convert.ToInt32(btnStall.Text);

            freeButtonStyle();
        }

        public bool isFreeStall()//повертає значення прапорця вільності місця
        {
            return isFree;
        }
        
        public void takePlace(float payment)//функція, що робить місце "зайнятим"
        {
            isFree = false;//змінити прапорець вільності

            this.payment = payment - PAYMENT_FOR_SERVICE;//зберегти внесену оплату, від якої при в`їзді віднімається стандартна платня за в`їзд на автостоянку

            engageButtonStyle();//змінити стиль кнопки
        }

        public void pauseTimer(bool isPaused)//зупини збільшення часу очікування в момент між виїздом з місця та виїздом зі стоянки
        {
            this.isPaused = isPaused;
        }

        public void freePlace()//функція, що "звільняє" місце
        {
            //скидання усіх полів в значення по замовчуванню
            waitTime = 0L;
            paidTime = 0L;
            isFree   = true;
            isPaused = false;
            payment  = 0.0f;

            freeButtonStyle();//зміна вигляду кнопки
        }

        public bool isEqualButton(Button btn)//перевіряє, чи передана кнопка рівня тій, що збережена в екземплярі класу
        {
            return btnStall.Equals(btn);
        }

        public float checkPayment()//функція, що підраховує кількість грошей до сплати за використання автостоянки
        {            
            payment -= (float)(waitTime - paidTime) * PAYMENT_COEFICIENT;//від внесеної платні при в`їзді віднімається добуток неоплаченого часу, проведеного на автостоянці на погодинний коефіціент платні
            
            paidTime = waitTime;
            
            return payment;
        }

        public void incrementWaitTime()//додає секунду до часу очікування, якщо це місце зайняте
        {
            if (!isFree && !isPaused)
            {
                waitTime++;

                refreshWaitTime();//оновлює текст на кнопці
            }
        }

        private void freeButtonStyle()//стиль кнопки, який відповідає "вільному" місцю на автостоянці
        {
            btnStall.Enabled = false;//кнопка недоступна
            btnStall.BackColor = Color.Yellow;//колір жовтий
            btnStall.Text = Convert.ToString(number);//в якості тесту виведено номер місця
        }

        private void engageButtonStyle()//стиль кнопки, який відповідає "зайнятому" місцю на автостоянці
        {
            btnStall.Enabled = true;//кнопка доступна для клікання
            btnStall.BackColor = Color.Red;//колір червоний
            refreshWaitTime();//оновлено значення часу очікування
        }  
        
        private void refreshWaitTime()
        {
            btnStall.Text = formatTime();//встановлення текстом кнопки відформатованого значення таймера
        }             

        private String formatTime()
        {
            return string.Format("{0:00}:{1:00}", waitTime / 60, waitTime % 60);//форматування кількості секунд (часу очікування) у вихляді "хвилини:секунди" 
        }

        private const float PAYMENT_COEFICIENT  = 0.003819f;//погодинний коефіціент
        private const float PAYMENT_FOR_SERVICE = 3.0f;//однорозова платня за користування послугами автостоянки
    }
}
