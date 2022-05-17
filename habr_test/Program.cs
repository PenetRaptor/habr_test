using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System.IO;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace habr_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //Переход между браузерами организовать в рамках одного приложения и в одном сеансе.//
            int caseSwitch;
            do
            {
                IWebDriver driver = null;
                FileInfo fileInf = new FileInfo("logfile.txt");
                List<IWebElement> linksToClick;

                Console.Write("Выберите браузер:\n" +
                    "1. Firefox\n" +
                    "2. Chrome\n" +
                    "0. Выход\n" +
                    "\n\nВведите номер функции => ");

                caseSwitch = Convert.ToInt32(Console.ReadLine());
                switch (caseSwitch)
                {
                    case 1: 
                        driver = new FirefoxDriver();
                        break;
                    case 2:
                        var chromeOptions = new ChromeOptions();
                        chromeOptions.AddArgument("log-level=3");
                        driver = new ChromeDriver(options: chromeOptions);
                        break;
                    default: Console.WriteLine("Данной задачи не существует.");
                        break;
                }

                ///////////////////////////////////////////////////////
                //Загрузить главную страницу сайта https://habr.com/.//
                driver.Navigate().GoToUrl("https://habr.com/");

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Пролистать все пункты главного меню (Все потоки, Разработка, Администрирование,Дизайн, Менеджмент, Маркетинг, Научпоп).//
                //По открытию страницы в log файл, а так же в окно консоли вывести заголовок каждой открытой страницы.                   //
                IWebElement element = driver.FindElement(By.ClassName("tm-main-menu__section-content"));

                foreach (var item in element.FindElements(By.TagName("a")))
                {
                    item.Click();
                    Console.WriteLine("Переход на пункт " + item.Text);
                    using (StreamWriter writer = new StreamWriter("logfile.txt", true))
                    {
                        writer.WriteLineAsync("Переход на пункт " + item.Text);
                    }
                }

                //////////////////////////////////
                //Вернуться на главную страницу.//
                element = driver.FindElement(By.ClassName("tm-header__logo-wrap"));
                element.Click();
                Console.WriteLine("Вернулись на главную");
                using (StreamWriter writer = new StreamWriter("logfile.txt", true))
                {
                    writer.WriteLineAsync("Вернулись на главную");
                }

                ///////////////////////////////////////////////////////////////
                //Получить список заголовков всех статей на главной странице.//
                //Список заголовков статей вывести в log и консоль.          //
                element = driver.FindElement(By.ClassName("tm-articles-list"));
                Console.WriteLine("Список статей:");
                foreach (var item in element.FindElements(By.ClassName("tm-article-snippet__title_h2")))
                {
                    Console.WriteLine(item.Text);
                    using (StreamWriter writer = new StreamWriter("logfile.txt", true))
                    {
                        writer.WriteLineAsync(item.Text);
                    }
                }

                /////////////////////////////////////////////////////////
                //Выполнить поиск по сайту с ключевым словом «selenium»//
                element = driver.FindElement(By.ClassName("tm-header-user-menu__search"));
                element.Click();

                element = driver.FindElement(By.ClassName("tm-input-text-decorated__input"));
                element.SendKeys("selenium");

                element = driver.FindElement(By.ClassName("tm-search__icon"));
                element.Click();

                //////////////////////////////////////////////////////////
                //В результатах поиска выполнить сортировку «По времени»//
                element = driver.FindElement(By.ClassName("tm-navigation-dropdown__button"));
                element.Click();

                linksToClick = driver.FindElement(By.ClassName("tm-navigation-dropdown__options")).FindElements(By.TagName("button")).ToList();
                linksToClick[1].Click();

                ///////////////////////////////////////////////////////////////////////////////////
                //В log и консоль вывести список хабов указанных под заголовком для первой статьи//
                new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementIsVisible(By.TagName("article")));

                linksToClick = driver.FindElements(By.TagName("article")).ToList();

                new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementIsVisible(By.ClassName("tm-article-snippet__hubs")));

                foreach (var item in linksToClick[0].FindElements(By.ClassName("tm-article-snippet__hubs-item-link")))
                {
                    Console.WriteLine(item.Text);
                    using (StreamWriter writer = new StreamWriter("logfile.txt", true))
                    {
                        writer.WriteLineAsync(item.Text);
                    }
                }

                //////////////////////////////////////////////////////////////////////////////////////
                //Для третьей статьи, вывести название статьи и текст кнопки «Читать далее»(возможны//
                //другие вариации) в log и консоль, и выполнить клик по ней.                        //
                using (StreamWriter writer = new StreamWriter("logfile.txt", true))
                {
                    writer.WriteLineAsync(linksToClick[2].FindElement(By.ClassName("tm-article-snippet__title_h2")).Text);
                    writer.WriteLineAsync(linksToClick[2].FindElement(By.ClassName("tm-article-snippet__readmore")).Text);
                }
                Console.WriteLine(linksToClick[2].FindElement(By.ClassName("tm-article-snippet__title_h2")).Text);
                Console.WriteLine(linksToClick[2].FindElement(By.ClassName("tm-article-snippet__readmore")).Text);
                element = linksToClick[2].FindElement(By.ClassName("tm-article-snippet__readmore"));
                element.Click();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Дождаться загрузки страницы. Вывести в log и консоль «Заголовок» статьи, и количество комментариев к ней//
                new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

                element = driver.FindElement(By.ClassName("tm-article-presenter"));
                using (StreamWriter writer = new StreamWriter("logfile.txt", true))
                {
                    writer.WriteLineAsync(element.FindElement(By.TagName("h1")).Text);
                    try
                    {
                        writer.WriteLineAsync(element.FindElement(By.ClassName("tm-comments-wrapper__comments-count")).Text);
                    }
                    catch (Exception)
                    {
                        writer.WriteLineAsync("Коментариев нет.");
                    }

                }
                Console.WriteLine(element.FindElement(By.TagName("h1")).Text);
                try
                {
                    Console.WriteLine(element.FindElement(By.ClassName("tm-comments-wrapper__comments-count")).Text);
                }
                catch (Exception)
                {
                    Console.WriteLine("Коментариев нет.");
                }
            } while (caseSwitch != 0);
        }
    }
}
