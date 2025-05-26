using DevExpress.Maui.Controls;
using DevExpress.Maui.Editors;
using Microsoft.Maui.Controls;
using MvvmHelpers;
using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using System.ComponentModel;
using CheckEdit = DevExpress.Maui.Editors.CheckEdit;
using ComboBoxEdit = DevExpress.Maui.Editors.ComboBoxEdit;
using DateEdit = DevExpress.Maui.Editors.DateEdit;
using FilterMode = DevExpress.Maui.Editors.FilterMode;
using MultilineEdit = DevExpress.Maui.Editors.MultilineEdit;
using Microsoft.Maui.Platform;
using MySqlConnector;
using System.Globalization;

namespace SmartPharma5.View;

[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class QuizForm2 : ContentPage
{
    public Partner_Form.Collection partner_Form { get; set; }
    public BindingList<Question> QuestionList { get; set; }
    public ObservableRangeCollection<Type_element> RequestList { get; set; }
    public ObservableRangeCollection<Response> ResponseList { get; set; }

    bool Edit_IsEnable { get; set; }
    public QuizForm2()
    {
        InitializeComponent();
        BindingContext = new QuizQuestionViewModel();
    }
    public QuizForm2(int id_partner_form)
    {
        InitializeComponent();
        BindingContext = new QuizQuestionViewModel(id_partner_form);
        var qvm = BindingContext as QuizQuestionViewModel;
        partner_Form = qvm.partner_form;

        Edit_IsEnable = !partner_Form.Validated && !(partner_Form.End_date < DateTime.Now);

        if (qvm != null)
        {
            QuestionList = qvm.QuestionList;
            RequestList = qvm.ElementList;
            ResponseList = qvm.ResponseList;
        }
        Title = partner_Form.Form_name;
        Partner_name.Text = partner_Form.Partner_name;
        Forms(QuestionList, RequestList);

        bool test = qvm.BtnShow;
        btnGroupe.IsVisible = Edit_IsEnable && test;
    }
    public QuizForm2(Partner_Form.Collection partner_form)
    {
        partner_Form = partner_form;

        try
        {
            BindingContext = new QuizQuestionViewModel(partner_Form);
            InitializeComponent();

            Edit_IsEnable = !partner_Form.Validated && !(partner_Form.End_date < DateTime.Now);

            var qvm = BindingContext as QuizQuestionViewModel;

            if (qvm != null)
            {
                try
                {
                    QuestionList = qvm.QuestionList;
                    RequestList = qvm.ElementList;
                    ResponseList = qvm.ResponseList;
                }
                catch (Exception ex)
                {

                }



            }
            Title = partner_Form.Form_name;
            Partner_name.Text = partner_Form.Partner_name;
            Forms(QuestionList, RequestList);

            btnGroupe.IsVisible = Edit_IsEnable;
        }
        catch (Exception ex)
        {

        }





    }
    /********************************/
    //private bool isSwipeOpen = false;

    //private void OnSwipeViewClicked(object sender, EventArgs e)
    //{
    //    if (isSwipeOpen)
    //    {
    //        MainSwipeView.Close();
    //    }
    //    else
    //    {
    //        MainSwipeView.Open(OpenSwipeItem.RightItems);
    //    }

    //    isSwipeOpen = !isSwipeOpen;
    //}
    private bool _refreshEnabled;
    private bool _firstAppear = true;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Activer le rafraîchissement périodique
        _refreshEnabled = true;

        // Exécuter le premier rafraîchissement
        await RefreshMessages();

        // Démarrer le timer pour les rafraîchissements périodiques
        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (!_refreshEnabled) return false;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await RefreshMessages();
            });
            return _refreshEnabled;
        });

        // Jouer l'animation seulement au premier affichage
        if (_firstAppear)
        {
            _firstAppear = false;
            await AnimateSwipeHint();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Désactiver le rafraîchissement périodique
        _refreshEnabled = false;
    }

    private async Task RefreshMessages()
    {
        try
        {
            int userId = Preferences.Get("iduser", 0);
            int unreadCount = await UserModel.GetUnreadMessagesCountAsync(
                userId,
                CurrentData.CurrentModuleId,
                CurrentData.CurrentNoteModule);

            Device.BeginInvokeOnMainThread(() =>
            {
                UnreadBadgeLabel.Text = unreadCount.ToString();
                UnreadBadgeFrame.IsVisible = unreadCount > 0;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du rafraîchissement: {ex.Message}");
        }
    }

    private async Task AnimateSwipeHint()
    {
        // Faire apparaître l'indicateur
        await swipeHint.FadeTo(0.8, 300);

        // Animation de balayage (2 cycles)
        for (int i = 0; i < 2; i++)
        {
            await swipeHint.TranslateTo(-20, 0, 300, Easing.SinInOut);
            await swipeHint.TranslateTo(0, 0, 300, Easing.SinInOut);
        }

        // Rendre l'indicateur plus discret
        await swipeHint.FadeTo(0.3, 300);
    }
    /********************************/
    /*public void Forms(BindingList<Question> questionList, ObservableRangeCollection<Type_element> requestlist)
    {


        foreach (Question question in questionList)
        {
            StackLayout stackLayoutQuestion = new StackLayout()
            {
                Padding = new Thickness(5),
            };
            StackLayout stackLayoutQuestionLabel = new StackLayout()
            {
                Padding = new Thickness(5),
            };
            StackLayout stackLayoutQuestionResponse = new StackLayout()
            {
                Padding = new Thickness(5),
            };
            Frame frame = new Frame()
            {
                Padding = new Thickness(10),
                BackgroundColor = Microsoft.Maui.Graphics.Colors.White
            };
            Label label = new Label();

            TextEdit TextAnswer = new TextEdit()
            {
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsReadOnly = !Edit_IsEnable
            };
            NumericEdit NumericAnswer = new NumericEdit()
            {
                Value = null,
                HeightRequest = 50,
                AllowNullValue = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsReadOnly = !Edit_IsEnable

            };
            CheckEdit CheckAnswer = new CheckEdit()
            {
                IsChecked = null,
                LabelFontSize = 30,
                CheckBoxPosition = CheckBoxPosition.End,

                IsEnabled = Edit_IsEnable


            };
            RadioButton RadioAnswer = new RadioButton()
            {
                FontSize = 18,
                IsEnabled = !Edit_IsEnable

            };
            DateEdit DateAnswer = new DateEdit()
            {
                Date = null,
                DisplayFormat = "yyyy/MM/dd",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsReadOnly = !Edit_IsEnable

            };
            ComboBoxEdit comboBoxEdit = new ComboBoxEdit()
            {
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsReadOnly = !Edit_IsEnable
            };
            MultilineEdit multilineEdit = new MultilineEdit()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsReadOnly = !Edit_IsEnable
            };
            SimpleButton buttonAdd = new SimpleButton()
            {
                BackgroundColor = Colors.Green,
                CornerRadius = 0,
                Margin = 2,
                Text = "+",
                HorizontalOptions = LayoutOptions.End,
                IsEnabled = Edit_IsEnable

            };
            SimpleButton buttonRemove = new SimpleButton()
            {
                Text = "-",
                BackgroundColor = Colors.Red,
                HorizontalOptions = LayoutOptions.End,
                IsEnabled = Edit_IsEnable
            };
            buttonRemove.Clicked += buttonRemoveClicked;
            buttonAdd.Clicked += buttonAddClicked;

            if (!question.Is_Null)
            {
                label = new Label
                {
                    Text = question.Question_Text + " *",
                    FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Colors.White,
                    TextColor = Colors.Black,
                };
            }
            else
            {
                label = new Label
                {
                    Text = question.Question_Text,
                    FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Colors.White,
                    TextColor = Colors.Black,
                    FontAttributes = FontAttributes.Bold,
                };
            }

            stackLayoutQuestionLabel.Children.Add(label);

            if (question.System_Type_Id == 1)
            {
                if (question.Input_editor == 1)
                {
                    if (!question.Is_Multi)
                    {
                        if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                        {


                            stackLayoutQuestionResponse.Children.Add(TextAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                        else
                        {

                            Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                            TextAnswer.Text = R.Response_String;

                            stackLayoutQuestionResponse.Children.Add(TextAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }

                    }
                    else
                    {

                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {
                            bool first = true;

                            List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                            foreach (Response response in R)
                            {
                                if (first)
                                {

                                    TextAnswer.Text = response.Response_String;
                                    stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                    first = false;
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }
                                else
                                {
                                    stackLayoutQuestionResponse = new StackLayout()
                                    {
                                        Padding = new Thickness(5),
                                        Orientation = StackOrientation.Horizontal,
                                    };
                                    TextAnswer = new TextEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        Text = response.Response_String,
                                        IsReadOnly = !Edit_IsEnable

                                    };
                                    buttonRemove = new SimpleButton()
                                    {
                                        Text = "-",
                                        BackgroundColor = Colors.Red,
                                        HorizontalOptions = LayoutOptions.End,
                                        IsEnabled = Edit_IsEnable
                                    };
                                    buttonRemove.Clicked += buttonRemoveClicked;


                                    stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }


                            }

                        }
                        else
                        {

                            stackLayoutQuestionResponse.Children.Add(TextAnswer);
                            stackLayoutQuestionResponse.Children.Add(buttonAdd);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                }
                else
                {
                    if (!question.Is_Multi)
                    {
                        if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                        {

                            stackLayoutQuestionResponse.Children.Add(multilineEdit);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                        else
                        {

                            Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                            multilineEdit.Text = R.Response_String;

                            stackLayoutQuestionResponse.Children.Add(multilineEdit);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                    else
                    {
                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {
                            bool first = true;

                            List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                            foreach (Response response in R)
                            {
                                if (first)
                                {

                                    multilineEdit.Text = response.Response_String;
                                    stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                    stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                    first = false;
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }
                                else
                                {
                                    stackLayoutQuestionResponse = new StackLayout()
                                    {
                                        Padding = new Thickness(5),
                                        Orientation = StackOrientation.Horizontal,
                                    };
                                    multilineEdit = new MultilineEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        Text = response.Response_String,
                                        IsReadOnly = !Edit_IsEnable
                                    };
                                    buttonRemove = new SimpleButton()
                                    {
                                        Text = "-",
                                        BackgroundColor = Colors.Red,
                                        HorizontalOptions = LayoutOptions.End,
                                        IsEnabled = Edit_IsEnable
                                    };
                                    buttonRemove.Clicked += buttonRemoveClicked;
                                    stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                    stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }


                            }
                        }
                        else
                        {
                            stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;

                            stackLayoutQuestionResponse.Children.Add(multilineEdit);
                            stackLayoutQuestionResponse.Children.Add(buttonAdd);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }

                    }
                }
            }
            else if (question.System_Type_Id == 2)
            {
                if (!question.Is_Multi)
                {
                    if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                    {

                        stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                    else
                    {

                        Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                        NumericAnswer.Value = R.Response_Int;

                        stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
                else
                {
                    stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                    if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                    {
                        bool first = true;
                        List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                        foreach (Response response in R)
                        {
                            if (first)
                            {

                                NumericAnswer.Value = response.Response_Int;
                                stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                first = false;
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                            else
                            {
                                stackLayoutQuestionResponse = new StackLayout()
                                {
                                    Padding = new Thickness(5),
                                    Orientation = StackOrientation.Horizontal,
                                };
                                NumericAnswer = new NumericEdit()
                                {
                                    HeightRequest = 50,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    Value = response.Response_Int,
                                    IsReadOnly = !Edit_IsEnable
                                };
                                buttonRemove = new SimpleButton()
                                {
                                    Text = "-",
                                    BackgroundColor = Colors.Red,
                                    HorizontalOptions = LayoutOptions.End,
                                    IsEnabled = Edit_IsEnable
                                };
                                buttonRemove.Clicked += buttonRemoveClicked;

                                stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }


                        }
                    }
                    else
                    {

                        stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                        stackLayoutQuestionResponse.Children.Add(buttonAdd);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }

                }
            }
            else if (question.System_Type_Id == 3)
            {

                if (!question.Is_Multi)
                {
                    if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                    {

                        stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                    else
                    {

                        Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                        NumericAnswer.Value = R.Response_Decimal;

                        stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
                else
                {
                    stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                    if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                    {
                        bool first = true;
                        List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();

                        foreach (Response response in R)
                        {
                            if (first)
                            {

                                NumericAnswer.Value = response.Response_Decimal;

                                stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                first = false;
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                            else
                            {
                                stackLayoutQuestionResponse = new StackLayout()
                                {
                                    Padding = new Thickness(5),
                                    Orientation = StackOrientation.Horizontal,
                                };
                                NumericAnswer = new NumericEdit()
                                {
                                    HeightRequest = 50,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    Value = response.Response_Decimal,
                                    IsReadOnly = !Edit_IsEnable,
                                };
                                buttonRemove = new SimpleButton()
                                {
                                    Text = "-",
                                    BackgroundColor = Colors.Red,
                                    HorizontalOptions = LayoutOptions.End,
                                    IsEnabled = Edit_IsEnable
                                };

                                buttonRemove.Clicked += buttonRemoveClicked;

                                stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }


                        }
                    }
                    else
                    {

                        stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                        stackLayoutQuestionResponse.Children.Add(buttonAdd);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
            }
            else if (question.System_Type_Id == 6)
            {


                if (!question.Is_Multi)
                {
                    if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                    {

                        stackLayoutQuestionResponse.Children.Add(DateAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                    else
                    {

                        Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();

                        DateAnswer.Date = R.Response_Date;
                        DateAnswer.IsReadOnly = !Edit_IsEnable;

                        stackLayoutQuestionResponse.Children.Add(DateAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
                else
                {
                    stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                    if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                    {
                        bool first = true;

                        List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                        foreach (Response response in R)
                        {
                            if (first)
                            {

                                DateAnswer.Date = response.Response_Date;
                                stackLayoutQuestionResponse.Children.Add(DateAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                first = false;
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                            else
                            {
                                stackLayoutQuestionResponse = new StackLayout()
                                {
                                    Padding = new Thickness(5),
                                    Orientation = StackOrientation.Horizontal,
                                };
                                DateAnswer = new DateEdit()
                                {
                                    HeightRequest = 50,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    DisplayFormat = "yyyy/MM/dd",
                                    Date = response.Response_Date,
                                    IsReadOnly = !Edit_IsEnable
                                };
                                buttonRemove = new SimpleButton()
                                {
                                    Text = "-",
                                    BackgroundColor = Colors.Red,
                                    HorizontalOptions = LayoutOptions.End,
                                    IsEnabled = Edit_IsEnable
                                };
                                buttonRemove.Clicked += buttonRemoveClicked;


                                stackLayoutQuestionResponse.Children.Add(DateAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }


                        }
                    }
                    else
                    {

                        stackLayoutQuestionResponse.Children.Add(DateAnswer);
                        stackLayoutQuestionResponse.Children.Add(buttonAdd);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
            }
            else if (question.System_Type_Id == 4)
            {
                if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                {
                    stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;

                    stackLayoutQuestionResponse.Children.Add(CheckAnswer);
                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                    frame.Content = stackLayoutQuestion;
                    Stacklay.Children.Add(frame);
                }
                else
                {

                    Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();

                    CheckAnswer.IsChecked = R.Response_Bool;


                    stackLayoutQuestionResponse.Children.Add(CheckAnswer);
                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                    frame.Content = stackLayoutQuestion;
                    Stacklay.Children.Add(frame);
                }
            }
            else
            {
                //----------------------------------------------------------
                ObservableRangeCollection<Type_element> list = new ObservableRangeCollection<Type_element>();
                foreach (Type_element Request in requestlist)
                {
                    if (question.System_Type_Id == Request.Type_id)
                    {
                        list.Add(Request);
                    }
                }


                if (question.Is_Multi)
                {
                    stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                    if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                    {
                        bool first = true;

                        List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                        foreach (Response response in R)
                        {
                            Frame frame1 = new Frame()
                            {
                                Padding = new Thickness(2)
                            };

                            StackLayout stackLayoutQuestionResponse1 = new StackLayout()
                            {
                                Padding = new Thickness(1),
                                Orientation = StackOrientation.Horizontal,
                            };

                            ComboBoxEdit comboBoxEdit1 = new ComboBoxEdit()
                            {
                                HeightRequest = 50,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                IsReadOnly = !Edit_IsEnable
                            };

                            StackLayout stackLayoutQuestion1 = new StackLayout()
                            {
                                Padding = new Thickness(5),
                            };
                            StackLayout stackLayoutQuestionLabel1 = new StackLayout()
                            {
                                Padding = new Thickness(5),
                            };


                            if (first)
                            {



                                first = false;
                                comboBoxEdit1.ItemsSource = list;
                                comboBoxEdit1.DisplayMember = "Name";
                                comboBoxEdit1.ValueMember = "Id";
                                comboBoxEdit1.IsFilterEnabled = true;
                                comboBoxEdit1.FilterMode = FilterMode.Contains;
                                comboBoxEdit1.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;
                                comboBoxEdit1.SelectedItem = list.Where(s => s.Id == response.Type_Element_Id).FirstOrDefault();

                                stackLayoutQuestionResponse1.Children.Add(comboBoxEdit1);
                                stackLayoutQuestionResponse1.Children.Add(buttonAdd);
                                stackLayoutQuestion1.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion1.Children.Add(stackLayoutQuestionResponse1);
                                frame1.Content = stackLayoutQuestion1;
                                Stacklay.Children.Add(frame1);
                            }
                            else
                            {


                                buttonRemove = new SimpleButton()
                                {
                                    Text = "-",
                                    BackgroundColor = Colors.Red,
                                    CornerRadius = 0,
                                    HorizontalOptions = LayoutOptions.End,
                                    IsEnabled = Edit_IsEnable,
                                    Margin = new Thickness(2),

                                };
                                buttonRemove.Clicked += buttonRemoveClicked;
                                comboBoxEdit1.Margin = 3;
                                comboBoxEdit1.ItemsSource = list;
                                comboBoxEdit1.BorderColor = Colors.CadetBlue;
                                comboBoxEdit1.DisplayMember = "Name";
                                comboBoxEdit1.ValueMember = "Id";
                                comboBoxEdit1.IsFilterEnabled = true;
                                comboBoxEdit1.FilterMode = FilterMode.Contains;
                                comboBoxEdit1.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;
                                comboBoxEdit1.SelectedItem = list.Where(s => s.Id == response.Type_Element_Id).FirstOrDefault();

                                stackLayoutQuestionResponse1.Children.Add(comboBoxEdit1);
                                stackLayoutQuestionResponse1.Children.Add(buttonRemove);
                                stackLayoutQuestion1.Children.Add(stackLayoutQuestionLabel1);
                                stackLayoutQuestion1.Children.Add(stackLayoutQuestionResponse1);
                                frame1.Content = stackLayoutQuestion1;
                                Stacklay.Children.Add(frame1);
                            }
                        }
                    }
                    else
                    {

                        comboBoxEdit = new ComboBoxEdit()
                        {
                            HeightRequest = 50,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            IsReadOnly = !Edit_IsEnable
                        };


                        comboBoxEdit.ItemsSource = list;
                        comboBoxEdit.DisplayMember = "Name";
                        comboBoxEdit.ValueMember = "Id";
                        comboBoxEdit.IsFilterEnabled = true;
                        comboBoxEdit.FilterMode = FilterMode.Contains;
                        comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;

                        stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                        stackLayoutQuestionResponse.Children.Add(buttonAdd);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
                else
                {
                    if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                    {

                        Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                        comboBoxEdit = new DevExpress.Maui.Editors.ComboBoxEdit()
                        {
                            HeightRequest = 50,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            IsReadOnly = !Edit_IsEnable
                        };

                        comboBoxEdit.ItemsSource = list;
                        comboBoxEdit.DisplayMember = "Name";
                        comboBoxEdit.ValueMember = "Id";
                        comboBoxEdit.IsFilterEnabled = true;
                        comboBoxEdit.FilterMode = FilterMode.Contains;
                        comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;
                        comboBoxEdit.SelectedItem = list.Where(s => s.Id == R.Type_Element_Id).FirstOrDefault();

                        stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                    else
                    {
                        comboBoxEdit = new ComboBoxEdit()
                        {
                            HeightRequest = 50,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            IsReadOnly = !Edit_IsEnable
                        };
                        comboBoxEdit.ItemsSource = list;
                        comboBoxEdit.DisplayMember = "Name";
                        comboBoxEdit.ValueMember = "Id";
                        comboBoxEdit.IsFilterEnabled = true;
                        comboBoxEdit.FilterMode = FilterMode.Contains;
                        comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;

                        stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }

                }

            }

        }
    }*/


    public void Forms(BindingList<Question> questionList, ObservableRangeCollection<Type_element> requestlist)
    {
        try
        {
            bool i = false;
            foreach (Question question in questionList)
            {
                StackLayout stackLayoutQuestion = new StackLayout()
                {


                    // BackgroundColor = i==true? Color.FromHex("#FAF9F9") : Color.FromHex("#E2EFF2"),


                    Padding = new Thickness(5),

                };

                StackLayout stackLayoutQuestionLabel = new StackLayout()
                {
                    Padding = new Thickness(5),
                    // BackgroundColor = i == true ? Color.FromHex("#FAF9F9") : Color.FromHex("#E2EFF2"),
                };

                StackLayout stackLayoutQuestionResponse = new StackLayout()
                {
                    Padding = new Thickness(5),
                };
                Frame frame = new Frame()
                {

                    Padding = new Thickness(5),
                    BackgroundColor = i == true ? Color.FromHex("#FAF9F9") : Colors.White
                    ,
                    BorderColor = Color.FromHex("#d9dbda")
                };

                Label label = new Label();

                TextEdit TextAnswer = new TextEdit()
                {
                    HeightRequest = 50,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    IsReadOnly = !Edit_IsEnable
                };
                NumericEdit NumericAnswer = new NumericEdit()
                {
                    Value = null,
                    HeightRequest = 50,
                    AllowNullValue = true,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    IsReadOnly = !Edit_IsEnable

                };
                CheckEdit CheckAnswer = new CheckEdit()
                {
                    IsChecked = null,
                    LabelFontSize = 30,
                    CheckBoxPosition = CheckBoxPosition.End,

                    IsEnabled = Edit_IsEnable


                };
                RadioButton RadioAnswer = new RadioButton()
                {
                    FontSize = 18,
                    IsEnabled = !Edit_IsEnable

                };
                DateEdit DateAnswer = new DateEdit()
                {
                    Date = null,
                    DisplayFormat = "yyyy/MM/dd",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    IsReadOnly = !Edit_IsEnable

                };
                ComboBoxEdit comboBoxEdit = new ComboBoxEdit()
                {
                    HeightRequest = 50,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    IsReadOnly = !Edit_IsEnable
                };
                MultilineEdit multilineEdit = new MultilineEdit()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    IsReadOnly = !Edit_IsEnable
                };
                SimpleButton buttonAdd = new SimpleButton()
                {
                    Margin = new Thickness(2),
                    Text = "+",
                    BackgroundColor = Colors.Green,
                    CornerRadius = 1,
                    HorizontalOptions = LayoutOptions.End,
                    IsEnabled = Edit_IsEnable

                };
                SimpleButton buttonRemove = new SimpleButton()
                {
                    Margin = new Thickness(2),
                    Text = "-",
                    BackgroundColor = Colors.Red,
                    CornerRadius = 1,
                    HorizontalOptions = LayoutOptions.End,
                    IsEnabled = Edit_IsEnable
                };
                buttonRemove.Clicked += buttonRemoveClicked;
                buttonAdd.Clicked += buttonAddClicked;

                if (!question.Is_Null)
                {
                    label = new Label
                    {
                        Text = question.Question_Text + " *",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = i == true ? Color.FromHex("#FAF9F9") : Colors.White,
                        TextColor = Colors.Black,
                    };
                }
                else
                {
                    label = new Label
                    {
                        Text = question.Question_Text,
                        FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = i == true ? Color.FromHex("#FAF9F9") : Colors.White,
                        TextColor = Colors.Black,
                    };
                }
                i = !i;

                stackLayoutQuestionLabel.Children.Add(label);

                if (question.System_Type_Id == 1)
                {
                    if (question.Input_editor == 1)
                    {
                        if (!question.Is_Multi)
                        {
                            if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                            {


                                stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                            else
                            {

                                Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                                TextAnswer.Text = R.Response_String;

                                stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }

                        }
                        else
                        {

                            stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                            if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                            {
                                bool first = true;

                                List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                                foreach (Response response in R)
                                {
                                    if (first)
                                    {

                                        TextAnswer.Text = response.Response_String;
                                        stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                        stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                        first = false;
                                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                        frame.Content = stackLayoutQuestion;
                                        Stacklay.Children.Add(frame);
                                    }
                                    else
                                    {
                                        stackLayoutQuestionResponse = new StackLayout()
                                        {
                                            Padding = new Thickness(5),
                                            Orientation = StackOrientation.Horizontal,
                                        };
                                        TextAnswer = new TextEdit()
                                        {
                                            HeightRequest = 50,
                                            HorizontalOptions = LayoutOptions.FillAndExpand,
                                            Text = response.Response_String,
                                            IsReadOnly = !Edit_IsEnable

                                        };
                                        buttonRemove = new SimpleButton()
                                        {
                                            Margin = new Thickness(2),
                                            Text = "-",
                                            CornerRadius = 1,
                                            BackgroundColor = Colors.Red,
                                            HorizontalOptions = LayoutOptions.End,
                                            IsEnabled = Edit_IsEnable
                                        };
                                        buttonRemove.Clicked += buttonRemoveClicked;


                                        stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                        stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                        //stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                        frame.Content = stackLayoutQuestion;
                                        //Stacklay.Children.Add(frame);
                                    }


                                }

                            }
                            else
                            {

                                stackLayoutQuestionResponse.Children.Add(TextAnswer);
                                stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                        }
                    }
                    else
                    {
                        if (!question.Is_Multi)
                        {
                            if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                            {

                                stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                            else
                            {

                                Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                                multilineEdit.Text = R.Response_String;

                                stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }
                        }
                        else
                        {
                            stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                            if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                            {
                                bool first = true;

                                List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                                foreach (Response response in R)
                                {
                                    if (first)
                                    {

                                        multilineEdit.Text = response.Response_String;
                                        stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                        stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                        first = false;
                                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                        frame.Content = stackLayoutQuestion;
                                        Stacklay.Children.Add(frame);
                                    }
                                    else
                                    {
                                        stackLayoutQuestionResponse = new StackLayout()
                                        {
                                            Padding = new Thickness(5),
                                            Orientation = StackOrientation.Horizontal,
                                        };
                                        multilineEdit = new MultilineEdit()
                                        {
                                            HeightRequest = 50,
                                            HorizontalOptions = LayoutOptions.FillAndExpand,
                                            Text = response.Response_String,
                                            IsReadOnly = !Edit_IsEnable
                                        };
                                        buttonRemove = new SimpleButton()
                                        {
                                            Margin = new Thickness(2),
                                            Text = "-",
                                            CornerRadius = 1,
                                            BackgroundColor = Colors.Red,
                                            HorizontalOptions = LayoutOptions.End,
                                            IsEnabled = Edit_IsEnable
                                        };
                                        buttonRemove.Clicked += buttonRemoveClicked;
                                        stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                        stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                        //stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                        frame.Content = stackLayoutQuestion;
                                        //Stacklay.Children.Add(frame);
                                    }


                                }
                            }
                            else
                            {
                                stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;

                                stackLayoutQuestionResponse.Children.Add(multilineEdit);
                                stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                frame.Content = stackLayoutQuestion;
                                Stacklay.Children.Add(frame);
                            }

                        }
                    }
                }
                else if (question.System_Type_Id == 2)
                {
                    if (!question.Is_Multi)
                    {
                        if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                        {

                            stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                        else
                        {

                            Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                            NumericAnswer.Value = R.Response_Int;

                            stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                    else
                    {
                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {
                            bool first = true;
                            List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                            foreach (Response response in R)
                            {
                                if (first)
                                {

                                    NumericAnswer.Value = response.Response_Int;
                                    stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                    first = false;
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }
                                else
                                {
                                    stackLayoutQuestionResponse = new StackLayout()
                                    {
                                        Padding = new Thickness(5),
                                        Orientation = StackOrientation.Horizontal,
                                    };
                                    NumericAnswer = new NumericEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        Value = response.Response_Int,
                                        IsReadOnly = !Edit_IsEnable
                                    };
                                    buttonRemove = new SimpleButton()
                                    {
                                        Margin = new Thickness(2),
                                        Text = "-",
                                        CornerRadius = 1,
                                        BackgroundColor = Colors.Red,
                                        HorizontalOptions = LayoutOptions.End,
                                        IsEnabled = Edit_IsEnable
                                    };
                                    buttonRemove.Clicked += buttonRemoveClicked;

                                    stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                    // stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    // Stacklay.Children.Add(frame);
                                }


                            }
                        }
                        else
                        {

                            stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                            stackLayoutQuestionResponse.Children.Add(buttonAdd);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }

                    }
                }
                else if (question.System_Type_Id == 3)
                {

                    if (!question.Is_Multi)
                    {
                        if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                        {

                            stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                        else
                        {

                            Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                            NumericAnswer.Value = R.Response_Decimal;

                            stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                    else
                    {
                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {
                            bool first = true;
                            List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();

                            foreach (Response response in R)
                            {
                                if (first)
                                {

                                    NumericAnswer.Value = response.Response_Decimal;

                                    stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                    first = false;
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }
                                else
                                {
                                    stackLayoutQuestionResponse = new StackLayout()
                                    {
                                        Padding = new Thickness(5),
                                        Orientation = StackOrientation.Horizontal,
                                    };
                                    NumericAnswer = new NumericEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        Value = response.Response_Decimal,
                                        IsReadOnly = !Edit_IsEnable,
                                    };
                                    buttonRemove = new SimpleButton()
                                    {
                                        Margin = new Thickness(2),
                                        Text = "-",
                                        CornerRadius = 1,
                                        BackgroundColor = Colors.Red,
                                        HorizontalOptions = LayoutOptions.End,
                                        IsEnabled = Edit_IsEnable
                                    };

                                    buttonRemove.Clicked += buttonRemoveClicked;

                                    stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                    // stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    // Stacklay.Children.Add(frame);
                                }


                            }
                        }
                        else
                        {

                            stackLayoutQuestionResponse.Children.Add(NumericAnswer);
                            stackLayoutQuestionResponse.Children.Add(buttonAdd);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                }
                else if (question.System_Type_Id == 6)
                {


                    if (!question.Is_Multi)
                    {
                        if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                        {

                            stackLayoutQuestionResponse.Children.Add(DateAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                        else
                        {

                            Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();

                            DateAnswer.Date = R.Response_Date;
                            DateAnswer.IsReadOnly = !Edit_IsEnable;

                            stackLayoutQuestionResponse.Children.Add(DateAnswer);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                    else
                    {
                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {
                            bool first = true;

                            List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                            foreach (Response response in R)
                            {
                                if (first)
                                {

                                    DateAnswer.Date = response.Response_Date;
                                    stackLayoutQuestionResponse.Children.Add(DateAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                    first = false;
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }
                                else
                                {
                                    stackLayoutQuestionResponse = new StackLayout()
                                    {
                                        Padding = new Thickness(5),
                                        Orientation = StackOrientation.Horizontal,
                                    };
                                    DateAnswer = new DateEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        DisplayFormat = "yyyy/MM/dd",
                                        Date = response.Response_Date,
                                        IsReadOnly = !Edit_IsEnable
                                    };
                                    buttonRemove = new SimpleButton()
                                    {
                                        Margin = new Thickness(2),
                                        CornerRadius = 0,
                                        Text = "-",
                                        BackgroundColor = Colors.Red,
                                        HorizontalOptions = LayoutOptions.End,
                                        IsEnabled = Edit_IsEnable
                                    };
                                    buttonRemove.Clicked += buttonRemoveClicked;


                                    stackLayoutQuestionResponse.Children.Add(DateAnswer);
                                    stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                    //  stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    // Stacklay.Children.Add(frame);
                                }


                            }
                        }
                        else
                        {

                            stackLayoutQuestionResponse.Children.Add(DateAnswer);
                            stackLayoutQuestionResponse.Children.Add(buttonAdd);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                }
                else if (question.System_Type_Id == 4)
                {
                    if (ResponseList.Count(r => r.QuestionId == question.Id) == 0)
                    {
                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;

                        stackLayoutQuestionResponse.Children.Add(CheckAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                    else
                    {

                        Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();

                        CheckAnswer.IsChecked = R.Response_Bool;


                        stackLayoutQuestionResponse.Children.Add(CheckAnswer);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                        stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                        frame.Content = stackLayoutQuestion;
                        Stacklay.Children.Add(frame);
                    }
                }
                else
                {
                    BindingList<Type_element> list = new BindingList<Type_element>();
                    if (question.By_ref == "")
                    {
                        foreach (Type_element Request in requestlist)
                        {
                            if (question.System_Type_Id == Request.Type_id)
                            {
                                list.Add(Request);
                            }
                        }

                    }
                    else
                    {
                        list = Type_element.GetElementByRequest(question.By_ref).Result;

                    }


                    if (question.Is_Multi)
                    {
                        stackLayoutQuestionResponse.Orientation = StackOrientation.Horizontal;
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {
                            bool first = true;

                            List<Response> R = ResponseList.Where(r => r.QuestionId == question.Id).ToList();
                            foreach (Response response in R)
                            {
                                if (first)
                                {
                                    comboBoxEdit = new ComboBoxEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        IsReadOnly = !Edit_IsEnable
                                    };


                                    first = false;
                                    comboBoxEdit.ItemsSource = list;
                                    comboBoxEdit.DisplayMember = "Name";
                                    comboBoxEdit.ValueMember = "Id";
                                    comboBoxEdit.IsFilterEnabled = true;
                                    comboBoxEdit.FilterMode = FilterMode.Contains;
                                    comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;
                                    comboBoxEdit.SelectedItem = list.Where(s => s.Id == response.Type_Element_Id).FirstOrDefault();

                                    stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                                    stackLayoutQuestionResponse.Children.Add(buttonAdd);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    Stacklay.Children.Add(frame);
                                }
                                else
                                {
                                    stackLayoutQuestionResponse = new StackLayout()
                                    {
                                        Padding = new Thickness(5),
                                        Orientation = StackOrientation.Horizontal,
                                    };
                                    comboBoxEdit = new ComboBoxEdit()
                                    {
                                        HeightRequest = 50,
                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                        IsReadOnly = !Edit_IsEnable
                                    };
                                    buttonRemove = new SimpleButton()
                                    {
                                        Margin = new Thickness(2),
                                        CornerRadius = 0,
                                        Text = "-",
                                        BackgroundColor = Colors.Red,
                                        HorizontalOptions = LayoutOptions.End,
                                        IsEnabled = Edit_IsEnable
                                    };
                                    buttonRemove.Clicked += buttonRemoveClicked;

                                    comboBoxEdit.ItemsSource = list;
                                    comboBoxEdit.DisplayMember = "Name";
                                    comboBoxEdit.ValueMember = "Id";
                                    comboBoxEdit.IsFilterEnabled = true;
                                    comboBoxEdit.BorderColor = Colors.CadetBlue;
                                    comboBoxEdit.FilterMode = FilterMode.Contains;
                                    comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;
                                    comboBoxEdit.SelectedItem = list.Where(s => s.Id == response.Type_Element_Id).FirstOrDefault();

                                    stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                                    stackLayoutQuestionResponse.Children.Add(buttonRemove);
                                    //stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                                    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                                    frame.Content = stackLayoutQuestion;
                                    // Stacklay.Children.Add(frame);
                                }
                            }
                        }
                        else
                        {

                            comboBoxEdit = new ComboBoxEdit()
                            {
                                HeightRequest = 50,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                IsReadOnly = !Edit_IsEnable
                            };


                            comboBoxEdit.ItemsSource = list;
                            comboBoxEdit.DisplayMember = "Name";
                            comboBoxEdit.ValueMember = "Id";
                            comboBoxEdit.IsFilterEnabled = true;
                            comboBoxEdit.FilterMode = FilterMode.Contains;
                            comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;

                            stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                            stackLayoutQuestionResponse.Children.Add(buttonAdd);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                    }
                    else
                    {
                        if (ResponseList.Count(r => r.QuestionId == question.Id) != 0)
                        {

                            Response R = ResponseList.Where(r => r.QuestionId == question.Id).FirstOrDefault();
                            comboBoxEdit = new DevExpress.Maui.Editors.ComboBoxEdit()
                            {
                                HeightRequest = 50,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                IsReadOnly = !Edit_IsEnable
                            };

                            comboBoxEdit.ItemsSource = list;
                            comboBoxEdit.DisplayMember = "Name";
                            comboBoxEdit.ValueMember = "Id";
                            comboBoxEdit.IsFilterEnabled = true;
                            comboBoxEdit.FilterMode = FilterMode.Contains;
                            comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;
                            comboBoxEdit.SelectedItem = list.Where(s => s.Id == R.Type_Element_Id).FirstOrDefault();

                            stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }
                        else
                        {
                            comboBoxEdit = new ComboBoxEdit()
                            {
                                HeightRequest = 50,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                IsReadOnly = !Edit_IsEnable
                            };
                            comboBoxEdit.ItemsSource = list;
                            comboBoxEdit.DisplayMember = "Name";
                            comboBoxEdit.ValueMember = "Id";
                            comboBoxEdit.IsFilterEnabled = true;
                            comboBoxEdit.FilterMode = FilterMode.Contains;
                            comboBoxEdit.FilterComparisonType = StringComparison.CurrentCultureIgnoreCase;

                            stackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                            stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                            frame.Content = stackLayoutQuestion;
                            Stacklay.Children.Add(frame);
                        }

                    }

                }
                //else
                //{
                //    stackLayoutQuestionLabel.Children.Add(label);
                //    if (question.Is_Multi)
                //    {
                //        foreach (Request_List Request in RequestList)
                //        {
                //            CheckAnswer = new CheckEdit();
                //            if (question.Id == Request.QuestionId)
                //            {

                //                        CheckAnswer.Label = Request.Name.ToString();
                //                stackLayoutQuestionResponse.Children.Add(CheckAnswer);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        foreach (Request_List Request in RequestList)
                //        {
                //            RadioAnswer = new RadioButton();
                //            if (question.Id == Request.QuestionId)
                //            {

                //                        RadioAnswer.Content = Request.Name.ToString();


                //                stackLayoutQuestionResponse.Children.Add(RadioAnswer);
                //            }
                //        }
                //    }
                //    stackLayoutQuestion.Children.Add(stackLayoutQuestionLabel);
                //    stackLayoutQuestion.Children.Add(stackLayoutQuestionResponse);
                //    frame.Content = stackLayoutQuestion;
                //    Stacklay.Children.Add(frame);
                //}
            }

        }
        catch (Exception ex)
        {

        }

    }
    private void buttonAddClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as SimpleButton;
            var stackLayoutQuestionResponse = button.Parent as StackLayout;
            var stackLayoutQuestion = stackLayoutQuestionResponse.Parent as StackLayout;
            SimpleButton buttonRemove = new SimpleButton()
            {
                Text = "-",
                BackgroundColor = Colors.Red,
                HorizontalOptions = LayoutOptions.End,
            };
            buttonRemove.Clicked += buttonRemoveClicked;




            StackLayout NewstackLayoutQuestionResponse = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(5), };

            var item = stackLayoutQuestionResponse.Children.ToList().First();
            System.Type t = item.GetType();
            if (t.Equals(typeof(TextEdit)))
            {
                TextEdit TextAnswer = new TextEdit()
                {
                    HeightRequest = 38,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                NewstackLayoutQuestionResponse.Children.Add(TextAnswer);
                NewstackLayoutQuestionResponse.Children.Add(buttonRemove);
            }
            else if (t.Equals(typeof(ComboBoxEdit)))
            {
                var LastItem = item as ComboBoxEdit;
                ComboBoxEdit comboBoxEdit = new ComboBoxEdit()
                {
                    HeightRequest = 50,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    ItemsSource = LastItem.ItemsSource,
                    DisplayMember = "Name",
                    ValueMember = "Id",
                    IsFilterEnabled = true,
                    FilterMode = FilterMode.Contains,
                    FilterComparisonType = StringComparison.CurrentCultureIgnoreCase,
                };
                //comboBoxEdit = item as ComboBoxEdit;
                NewstackLayoutQuestionResponse.Children.Add(comboBoxEdit);
                NewstackLayoutQuestionResponse.Children.Add(buttonRemove);
            }
            else if (t.Equals(typeof(DateEdit)))
            {
                var LastItem = item as DateEdit;
                DateEdit DateEdit = new DateEdit()
                {
                    Date = DateTime.Now,
                    DisplayFormat = "yyyy/MM/dd",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                //comboBoxEdit = item as ComboBoxEdit;
                NewstackLayoutQuestionResponse.Children.Add(DateEdit);
                NewstackLayoutQuestionResponse.Children.Add(buttonRemove);
            }
            else if (t.Equals(typeof(MultilineEdit)))
            {
                var LastItem = item as MultilineEdit;
                MultilineEdit MultilineEdit = new MultilineEdit()
                {

                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                //comboBoxEdit = item as ComboBoxEdit;
                NewstackLayoutQuestionResponse.Children.Add(MultilineEdit);
                NewstackLayoutQuestionResponse.Children.Add(buttonRemove);
            }
            else
            {
                NumericEdit NumericAnswer = new NumericEdit()
                {
                    HeightRequest = 38,
                    AllowNullValue = true,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                NewstackLayoutQuestionResponse.Children.Add(NumericAnswer);
                NewstackLayoutQuestionResponse.Children.Add(buttonRemove);
            }
            stackLayoutQuestion.Children.Add(NewstackLayoutQuestionResponse);

        }
        catch (Exception ex)
        {

        }

    }

    private void buttonRemoveClicked(object sender, EventArgs e)
    {
        var button = sender as SimpleButton;
        var stackLayoutQuestionResponse = button.Parent as StackLayout;
        var stackLayoutQuestion = stackLayoutQuestionResponse.Parent as StackLayout;
        stackLayoutQuestion.Children.Remove(stackLayoutQuestionResponse);
    }


    private async void Submit_Clicked(object sender, EventArgs e)
    {
        try
        {
            SavingPopup.IsOpen = true;
            await Task.Delay(100);
            Partner_Form.UpdateMacAdresse(partner_Form.Id);
            int i = 0;
            bool isTrue = true;
            var frameList = Stacklay.Children.ToList();
            List<Response> List = new List<Response>();
            foreach (Question question in QuestionList)
            {
                if (!isTrue)
                    break;
                var frame2 = frameList[i] as Frame;
                var stackLayoutQuestion = frame2.Content as StackLayout;

                //if (question.Question_Type_Id == 2)
                //{
                if (!question.Is_Multi)
                {

                    var stackLayoutQuestionResponse = stackLayoutQuestion.Children.Last() as StackLayout;
                    var item = stackLayoutQuestionResponse.Children.First();

                    System.Type t = item.GetType();
                    if (t.Equals(typeof(TextEdit)))
                    {
                        var TextAnswer = item as TextEdit;
                        if (!question.Is_Null && (TextAnswer.Text == null || TextAnswer.Text == string.Empty))
                        {
                            TextAnswer.HasError = true;
                            await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                            isTrue = false;
                            break;
                        }
                        else
                        {
                            TextAnswer.HasError = false;
                            if (TextAnswer.Text != null)
                                List.Add(new Response(question.Id, TextAnswer.Text.ToString(), null, null, null, null, null));
                        }

                    }
                    else if (t.Equals(typeof(MultilineEdit)))
                    {
                        var multilineEdit = item as MultilineEdit;
                        if (!question.Is_Null && (multilineEdit.Text == null || multilineEdit.Text == string.Empty))
                        {
                            multilineEdit.HasError = true;
                            await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                            isTrue = false;
                            break;
                        }
                        else
                        {
                            multilineEdit.HasError = false;
                            if (multilineEdit.Text != null)
                                List.Add(new Response(question.Id, multilineEdit.Text.ToString(), null, null, null, null, null));
                        }

                    }
                    else if (t.Equals(typeof(NumericEdit)))
                    {
                        var NumericAnswer = item as NumericEdit;
                        if (!question.Is_Null && NumericAnswer.Value == null)
                        {
                            NumericAnswer.HasError = true;
                            await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                            isTrue = false;
                            break;
                        }
                        else
                        {
                            NumericAnswer.HasError = false;

                            if (NumericAnswer.Value != null)
                            {
                                if (question.System_Type_Id == 2)
                                    List.Add(new Response(question.Id, "", null, (int)NumericAnswer.Value, null, null, null));
                                else
                                    List.Add(new Response(question.Id, "", null, null, (decimal)NumericAnswer.Value, null, null));
                            }
                        }

                    }
                    else if (t.Equals(typeof(DateEdit)))
                    {
                        var DateAnswer = item as DateEdit;
                        List.Add(new Response(question.Id, "", null, null, null, DateAnswer.Date, null));
                    }
                    else if (t.Equals(typeof(CheckEdit)))
                    {
                        var CheckAnswer = item as CheckEdit;
                        if (CheckAnswer != null)
                            List.Add(new Response(question.Id, "", null, null, null, null, CheckAnswer.IsChecked));

                    }
                    else if (t.Equals(typeof(ComboBoxEdit)))
                    {
                        var ComboBoxAnswer = item as ComboBoxEdit;
                        if (!question.Is_Null && ComboBoxAnswer.SelectedValue == null)
                        {
                            ComboBoxAnswer.HasError = true;
                            await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                            isTrue = false;
                            break;
                        }
                        else if (ComboBoxAnswer.SelectedValue != null)
                        {
                            ComboBoxAnswer.HasError = false;
                            List.Add(new Response(question.Id, "", (uint)ComboBoxAnswer.SelectedValue, null, null, null, null));
                        }
                    }
                }
                else
                {
                    int responsecount = stackLayoutQuestion.Children.Count() - 1;
                    var responseList = stackLayoutQuestion.Children.ToList();
                    for (int j = 1; j <= responsecount; j++)
                    {
                        var stackLayoutQuestionResponse = responseList[j] as StackLayout;
                        var item = stackLayoutQuestionResponse.Children.First();
                        System.Type t = item.GetType();
                        if (t.Equals(typeof(TextEdit)))
                        {
                            var TextAnswer = item as TextEdit;
                            if (!question.Is_Null && (TextAnswer.Text == null || TextAnswer.Text == string.Empty))
                            {
                                TextAnswer.HasError = true;
                                await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                                isTrue = false;
                                break;
                            }
                            else
                            {
                                TextAnswer.HasError = false;
                                if (TextAnswer.Text != null)
                                    List.Add(new Response(question.Id, TextAnswer.Text.ToString(), null, null, null, null, null));
                            }

                        }
                        else if (t.Equals(typeof(NumericEdit)))
                        {
                            var NumericAnswer = item as NumericEdit;
                            if (!question.Is_Null && NumericAnswer.Value == null)
                            {
                                NumericAnswer.HasError = true;
                                await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                                isTrue = false;
                                break;
                            }
                            else
                            {
                                NumericAnswer.HasError = false;
                                if (NumericAnswer.Value != null)
                                {
                                    if (question.System_Type_Id == 2)
                                        List.Add(new Response(question.Id, "", null, (int)NumericAnswer.Value, null, null, null));
                                    else
                                        List.Add(new Response(question.Id, "", null, null, (decimal)NumericAnswer.Value, null, null));
                                }
                            }

                        }
                        else if (t.Equals(typeof(DateEdit)))
                        {

                            var DateAnswer = item as DateEdit;
                            List.Add(new Response(question.Id, "", null, null, null, DateAnswer.Date, null));
                        }
                        else if (t.Equals(typeof(MultilineEdit)))
                        {
                            var multilineEdit = item as MultilineEdit;
                            if (!question.Is_Null && (multilineEdit.Text == null || multilineEdit.Text == string.Empty))
                            {
                                multilineEdit.HasError = true;
                                await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                                isTrue = false;
                                break;
                            }
                            else
                            {
                                multilineEdit.HasError = false;
                                if (multilineEdit.Text != null)
                                    List.Add(new Response(question.Id, multilineEdit.Text.ToString(), null, null, null, null, null));
                            }

                        }
                        else if (t.Equals(typeof(CheckEdit)))
                        {

                            var CheckAnswer = item as CheckEdit;
                            if (!question.Is_Null && CheckAnswer.IsChecked == null)
                            {
                                CheckAnswer.BackgroundColor = Colors.Red;
                                StackLayout stack = CheckAnswer.Parent as StackLayout;

                                StackLayout stack1 = stack.Parent as StackLayout;

                                Frame frame = stack1.Parent as Frame;
                                frame.BorderColor = Colors.Red;
                                await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                                isTrue = false;
                                break;

                            }
                            else
                            {

                                CheckAnswer.BackgroundColor = Colors.White;
                                StackLayout stack = CheckAnswer.Parent as StackLayout;

                                StackLayout stack1 = stack.Parent as StackLayout;

                                Frame frame = stack1.Parent as Frame;
                                frame.BorderColor = Colors.GreenYellow;

                                List.Add(new Response(question.Id, "", null, null, null, null, CheckAnswer.IsChecked));

                            }
                        }
                        else if (t.Equals(typeof(ComboBoxEdit)))
                        {
                            var ComboBoxAnswer = item as ComboBoxEdit;
                            if (!question.Is_Null && ComboBoxAnswer.SelectedValue == null)
                            {
                                ComboBoxAnswer.HasError = true;
                                await ScrolView.ScrollToAsync(frame2, ScrollToPosition.Center, true);
                                isTrue = false;
                                break;
                            }
                            else if (ComboBoxAnswer.SelectedValue != null)
                            {
                                ComboBoxAnswer.HasError = false;
                                List.Add(new Response(question.Id, "", (uint)ComboBoxAnswer.SelectedValue, null, null, null, null));
                            }
                        }
                    }
                }
                //}
                //else
                //{
                //    var stackLayoutQuestionResponse = stackLayoutQuestion.Children.Last() as StackLayout;
                //    int responsecount = stackLayoutQuestionResponse.Children.Count();
                //    var responseList = stackLayoutQuestionResponse.Children.ToList();
                //    bool ischecked = false;
                //    for (int j = 0; j < responsecount; j++)
                //    {
                //        //var stackLayoutQuestionResponse = responseList[j] as StackLayout;
                //        var item = responseList[j];

                //        Type t = item.GetType();
                //        if (t.Equals(typeof(CheckEdit)))
                //        {
                //            var CheckAnswer = item as CheckEdit;
                //            if (CheckAnswer.IsChecked == true)
                //            {
                //                ischecked = true;

                //                if (question.Response_Type_Id == 1)
                //                    List.Add(new Response(CheckAnswer.Label.ToString(), null, null, null, question.Id));
                //                else if (question.Response_Type_Id == 2)
                //                    List.Add(new Response("", int.Parse(CheckAnswer.Label.ToString()), null, null, question.Id));
                //                else if (question.Response_Type_Id == 4)
                //                    List.Add(new Response("", null, (double)int.Parse(CheckAnswer.Label.ToString()), null, question.Id));
                //                else if (question.Response_Type_Id == 6)
                //                    List.Add(new Response("", null, null, DateTime.Parse(CheckAnswer.Label.ToString()), question.Id));
                //            }
                //        }
                //        if (t.Equals(typeof(RadioButton)))
                //        {
                //            var RadioAnswer = item as RadioButton;
                //            if (RadioAnswer.IsChecked == true)
                //            {
                //                ischecked = true;

                //                if (question.Response_Type_Id == 1)
                //                    List.Add(new Response(RadioAnswer.Content.ToString(), null, null, null, question.Id));
                //                else if (question.Response_Type_Id == 2)
                //                    List.Add(new Response("", int.Parse(RadioAnswer.Content.ToString()), null, null, question.Id));
                //                else if (question.Response_Type_Id == 4)
                //                    List.Add(new Response("", null, (double)int.Parse(RadioAnswer.Content.ToString()), null, question.Id));
                //                else if (question.Response_Type_Id == 6)
                //                    List.Add(new Response("", null, null, DateTime.Parse(RadioAnswer.Content.ToString()), question.Id));
                //            }
                //        }
                //        if (j == responsecount-1 && !ischecked && !question.Is_Null)
                //        {
                //            ScrolView.ScrollToAsync(frame2, Xamarin.Forms.ScrollToPosition.Center, true);
                //            frame2.BorderColor = Color.Red;
                //            isTrue = false;
                //            break;
                //        }
                //        else
                //        {
                //            frame2.BorderColor = Color.White;
                //        }
                //    }
                //}
                i++;
            }
            if (isTrue)
            {
                if (DbConnection.ConnectionIsTrue())
                {
                    try
                    {


                        var p = Task.Run(() => Response.Insert(List, partner_Form));
                        await p;
                        var c = Task.Run(() => Partner_Form.Update(partner_Form));
                        await c;
                        SavingPopup.IsOpen = false;
                        SuccessPopup.IsOpen = true;
                        await Task.Delay(1000);
                        SuccessPopup.IsOpen = false;
                        await App.Current.MainPage.Navigation.PopAsync();
                        await App.Current.MainPage.Navigation.PopAsync();
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "OK");
                    }
                }
                else
                    await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");

            }
            SavingPopup.IsOpen = false;

        }
        catch (Exception ex)
        {

        }

    }

    private async void SimpleButton_Clicked(object sender, EventArgs e)
    {
        var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
        if (r)
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }
    }


    /*****************log**************************/
    public async Task Log(string actionType)
    {
        string macaddress = DbConnection.macAdresse();
        string ipaddress = DbConnection.IpAddress();
        string machinename = DbConnection.MachineName();
        string gpsCoordinates = null;

        try
        {
            // Tentative de récupération de la localisation avec timeout
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            Location location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                gpsCoordinates = $"{location.Latitude.ToString("F15", CultureInfo.InvariantCulture)},{location.Longitude.ToString("F15", CultureInfo.InvariantCulture)}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur de géolocalisation (non bloquante) : " + ex.Message);
            // On continue sans les coordonnées GPS
        }

        try
        {
            string sqlCmd = @"INSERT INTO atooerp_log 
                     SET user = " + Preferences.Get("iduser", 0) +
                                 ", object_type = '" + CurrentData.CurrentNoteModule.Replace("'", "''") +
                                 "', object = " + CurrentData.CurrentModuleId +
                                 ", type = '" + actionType.Replace("'", "''") +
                                 "', name = '" + ("quiz form").Replace("'", "''") + "'" +
                                 ", date = NOW()" +
                                 ", ip = '" + ipaddress.Replace("'", "''") + "'" +
                                 (gpsCoordinates != null ? ", gps = '" + gpsCoordinates.Replace("'", "''") + "'" : "") +
                                 ", machine_name = '" + machinename.Replace("'", "''") + "'" +
                                 ", mac_adress = '" + macaddress.Replace("'", "''") + "'";

            MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con);
            DbConnection.Connecter();

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur SQL lors de l'enregistrement du log : " + ex.Message);
                Console.WriteLine("Requête SQL: " + sqlCmd); // Pour débogage
            }
            finally
            {
                DbConnection.Deconnecter();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur générale lors de l'enregistrement du log : " + ex.Message);
        }
    }
    /**********************************************/
    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        SavingPopup.IsOpen = true;
        await Task.Delay(100);

        //Console.WriteLine(Stacklay.Children.Count);


        int i = 0;
        var frameList = Stacklay.Children.ToList();
        List<Response> List = new List<Response>();
        foreach (Question question in QuestionList)
        {
            var frame2 = frameList[i] as Frame;
            var stackLayoutQuestion = frame2.Content as StackLayout;

            if (!question.Is_Multi)
            {

                var stackLayoutQuestionResponse = stackLayoutQuestion.Children.Last() as StackLayout;
                var item = stackLayoutQuestionResponse.Children.First();

                System.Type t = item.GetType();
                if (t.Equals(typeof(TextEdit)))
                {
                    var TextAnswer = item as TextEdit;
                    if (TextAnswer.Text != null && TextAnswer.Text != string.Empty)
                        List.Add(new Response(question.Id, TextAnswer.Text.ToString(), null, null, null, null, null));

                }
                else if (t.Equals(typeof(MultilineEdit)))
                {
                    var multilineEdit = item as MultilineEdit;
                    if (multilineEdit.Text != null && multilineEdit.Text != string.Empty)
                        List.Add(new Response(question.Id, multilineEdit.Text.ToString(), null, null, null, null, null));

                }
                else if (t.Equals(typeof(NumericEdit)))
                {
                    var NumericAnswer = item as NumericEdit;
                    if (NumericAnswer.Value != null)
                    {
                        if (question.System_Type_Id == 2)
                            List.Add(new Response(question.Id, "", null, (int)NumericAnswer.Value, null, null, null));
                        else
                            List.Add(new Response(question.Id, "", null, null, (decimal)NumericAnswer.Value, null, null));
                    }

                }
                else if (t.Equals(typeof(DateEdit)))
                {
                    var DateAnswer = item as DateEdit;
                    List.Add(new Response(question.Id, "", null, null, null, DateAnswer.Date, null));
                }
                else if (t.Equals(typeof(CheckEdit)))
                {
                    var CheckAnswer = item as CheckEdit;
                    if (CheckAnswer != null)
                        List.Add(new Response(question.Id, "", null, null, null, null, CheckAnswer.IsChecked));

                }
                else if (t.Equals(typeof(ComboBoxEdit)))
                {
                    var ComboBoxAnswer = item as ComboBoxEdit;
                    if (ComboBoxAnswer.SelectedValue != null)
                        List.Add(new Response(question.Id, "", (uint)ComboBoxAnswer.SelectedValue, null, null, null, null));
                }
            }
            else
            {
                int responsecount = stackLayoutQuestion.Children.Count() - 1;
                var responseList = stackLayoutQuestion.Children.ToList();
                for (int j = 1; j <= responsecount; j++)
                {
                    var stackLayoutQuestionResponse = responseList[j] as StackLayout;
                    var item = stackLayoutQuestionResponse.Children.First();
                    System.Type t = item.GetType();
                    if (t.Equals(typeof(TextEdit)))
                    {
                        var TextAnswer = item as TextEdit;
                        if (TextAnswer.Text != null && TextAnswer.Text != string.Empty)
                            List.Add(new Response(question.Id, TextAnswer.Text.ToString(), null, null, null, null, null));

                    }
                    else if (t.Equals(typeof(NumericEdit)))
                    {
                        var NumericAnswer = item as NumericEdit;
                        if (NumericAnswer.Value != null)
                        {
                            if (question.System_Type_Id == 2)
                                List.Add(new Response(question.Id, "", null, (int)NumericAnswer.Value, null, null, null));
                            else
                                List.Add(new Response(question.Id, "", null, null, (decimal)NumericAnswer.Value, null, null));
                        }

                    }
                    else if (t.Equals(typeof(DateEdit)))
                    {
                        var DateAnswer = item as DateEdit;
                        List.Add(new Response(question.Id, "", null, null, null, DateAnswer.Date, null));
                    }
                    else if (t.Equals(typeof(MultilineEdit)))
                    {
                        var multilineEdit = item as MultilineEdit;
                        if (multilineEdit.Text != null && multilineEdit.Text != string.Empty)
                        {

                            List.Add(new Response(question.Id, multilineEdit.Text.ToString(), null, null, null, null, null));
                        }

                    }
                    else if (t.Equals(typeof(CheckEdit)))
                    {

                        var CheckAnswer = item as CheckEdit;
                        if (CheckAnswer != null)
                            List.Add(new Response(question.Id, "", null, null, null, null, CheckAnswer.IsChecked));
                    }
                    else if (t.Equals(typeof(ComboBoxEdit)))
                    {

                        var ComboBoxAnswer = item as ComboBoxEdit;
                        if (ComboBoxAnswer.SelectedValue != null)
                            List.Add(new Response(question.Id, "", (uint)ComboBoxAnswer.SelectedValue, null, null, null, null));

                    }
                }
            }
            i++;
        }

        if (DbConnection.ConnectionIsTrue())
        {
            try
            {

                await Task.Delay(100);
                var p = Task.Run(() => Response.Insert(List, partner_Form));
                await p;
                Partner_Form.UpdateOpenDate(partner_Form);
                SavingPopup.IsOpen = false;
                SuccessPopup.IsOpen = true;
                await Task.Delay(1000);
                SuccessPopup.IsOpen = false;
                Partner_Form.UpdateMacAdresse(partner_Form.Id);
                // Nouveau: Appel de la méthode de log
                try
                {
                    await Log("update"); // ou une autre actionType appropriée
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du logging: " + ex.Message);
                    // Ne pas bloquer le flux même si le logging échoue
                }
                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "OK");
            }
        }
        else
            await App.Current.MainPage.DisplayAlert("Warning", "Connection failed", "Ok");

        SavingPopup.IsOpen = false;

    }


    private async void OnActionButtonClickedmemo(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("note3.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.MemoView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

        }
    private async void OnActionButtonClickedactivity(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        //UserDialogs.Instance.ShowLoading("Chargement...");
        //await Task.Delay(200);
        if (button.Source.ToString().Contains("activity.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ActivityView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        //UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedchat(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Chargement...");
        await Task.Delay(400);
        if (button.Source.ToString().Contains("chat.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ChatView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickeddocument(object sender, EventArgs e)
    {
        UserDialogs.Instance.ShowLoading("Loading...");

        try
        {
            var partnerform = this.partner_Form; // Assurez-vous que cette propriété est accessible

            // Appeler la même navigation que l'ancien bouton
            await Navigation.PushAsync(new FileSelectionView(partnerform.Id, EntityType.Partner_Form));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Une erreur est survenue: {ex.Message}", "OK");
        }
        finally
        {
            UserDialogs.Instance.HideLoading();
        }
    }
    private async void OnActionButtonClickedreport(object sender, EventArgs e)
    {
        var button = sender as ImageButton;

        UserDialogs.Instance.ShowLoading("Loading...");
        await Task.Delay(200);
        if (button.Source.ToString().Contains("rapport.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.ReportView());
        }
        else
        {
            /* string buttonName = button.Source.ToString().Replace("File: ", "");
             await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");*/
        }
        UserDialogs.Instance.HideLoading();

    }
    private async void OnActionButtonClickedlogs(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button.Source.ToString().Contains("logs.png"))
        {
            await Navigation.PushAsync(new FloatingActionButton.LogsView());
        }
        else
        {
            string buttonName = button.Source.ToString().Replace("File: ", "");
            await DisplayAlert("Action", $"Vous avez cliqué sur {buttonName}", "OK");
        }
    }
}