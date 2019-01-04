/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 04.01.2019
 * Время: 19:32
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using AI.MathMod;
using AI.MathMod.ML.NeuronNetwork; // Нейросети
using AI.MathMod.AdditionalFunctions; // Доп. ф-ии
using AI.MathMod.Signals; // Работа с цифровыми сигналами

namespace NeuralNet
{
	/// <summary>
	/// Description of NetClassificationExemple.
	/// </summary>
	public class NetClassificationExemple
	{
		public NetClassificationExemple()
		{
			GetNet(t.N, 3); // генерация нейронной сети
		}
		
		Net net;
		Vector t = MathFunc.GenerateTheSequence(0, 0.005, 1); // отсчеты времени
		
		
		/// <summary>
		/// Обучение нейронной сети
		/// </summary>
		public void Train()
		{
			// Обучение и тестирование на 23 эпохи
			for (int i = 0; i < 200; i++)
			{
				Console.WriteLine("Эпоха №{0}: ", i+1); // Вывод номера эпохи
				Train( 5); // Побучение  по 5 итерациям, по каждому классу
				if(Test(170) > 99.9) break; // Тестирование на 170 примерах
			}
		}
		
		
		/// <summary>
		/// Получение метки класса в результате работы нейронки
		/// </summary>
		/// <param name="sig">Сигнал</param>
		/// <returns>Метка класса</returns>
		public int GetClass(Vector sig)
		{
			Vector outp = net.Output(sig); // Получаем выходной вектор с нейросети
			double max = Statistic.MaximalValue(outp); // ищем максимальную активацию
			
			return (int)outp.IndexValue(max); // Ищем индекс (это и есть наш класс)
		}
		
		
		/// <summary>
		/// Обучение нейронной сети
		/// </summary>
		/// <param name="n">Количество итераций обучения</param>
		void Train(int n)
		{
			Random rnd = new Random();
			
			Vector sig; // Отсчеты сигнала
			double eps = 0; // Ошибка
			
			for (int i = 0; i < n; i++)
			{
				sig = Signal.Sin(t, 3)+Statistic.randNorm(t.N, rnd); // Шум + синус частотой 3 Гц
				eps += net.TrainClassifier(sig, 0); // Обучение НС запись ошибки класс 1
				sig = Signal.Rect(t, 3)+Statistic.randNorm(t.N, rnd); // Шум + меандр с частотой 3 Гц
				eps += net.TrainClassifier(sig, 1); // Обучение НС запись ошибки класс 2
				sig = Statistic.randNorm(t.N, rnd); // Просто шум
				eps += net.TrainClassifier(sig, 2); // Обучение НС запись ошибки класс 3
			}
			
			Console.WriteLine("Обучение на выборке {0} элементов, средняя ошибка {1}", n, eps/(3*n));
		}
	
		
		/// <summary>
		/// Тестирование нейронной сети
		/// </summary>
		/// <param name="n">размер тестовой выборки</param>
		public double Test(int n)
		{
			Random rnd = new Random();
			
			Vector sig; // Отсчеты сигнала
			double yes = 0, acc; // правильно распознанные классы
			
			
			for (int i = 0; i < n; i++)
			{
				sig = Signal.Sin(t, 3)+Statistic.randNorm(t.N, rnd); // Шум + синус частотой 3 Гц
				if(GetClass(sig) == 0) yes ++; // Тестирование класса 1
				sig = Signal.Rect(t, 3)+Statistic.randNorm(t.N, rnd); // Шум + меандр с частотой 3 Гц
				if(GetClass(sig) == 1) yes ++; // Тестирование класса 2
				sig = Statistic.randNorm(t.N, rnd); // Просто шум
				if(GetClass(sig) == 2) yes ++; // Тестирование класса 3
			}	
			
			acc =  100*(yes/(3*n));
			
			Console.WriteLine("Тестирование на выборке {0} элементов, точность {1}%", n, acc);
			
			return acc;
		}
		
	
		
		
		/// <summary>
		/// Создание нейронной сети
		/// </summary>
		/// <param name="inpDim">Количество входов</param>
		/// <param name="classCount">Количествой классов</param>
		/// <returns>Нейросеть</returns>
		void GetNet(int inpDim, int classCount)
		{
			net = new Net(); // Создание нейросети
			net.Add(new Sigmoid(inpDim, 10)); // добавление сигмоидального скрытого слоя с 10 нейронами
			net.Add(new Softmax(classCount)); // Выходной софтмакс слой
			Console.WriteLine("Learning Rate: {0}", net.LerningRate); // Вывод рассчитанной скорости обучение
			net.Moment = 0.8; // Установка момента
		}
		
		
	}
}
	
