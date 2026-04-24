using BlazorMonaco.Editor;
using FrontEnd_BlazorApp.Models.JsFunc;
using FrontEnd_BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FrontEnd_BlazorApp.Pages.Workshop.JsFunc
{
    public partial class Editor
    {
        [Inject]
        public IGoLogixService goLogixService { get;set; }
        [Inject]
        public IMessageService message {get;set; }

        [Parameter]
        public string? Id { get; set; }



        StandaloneCodeEditor _editor;
        Models.JsFunc.JsConfig cfg;

        private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "javascript",
                Value = _curFunc.FunctionBody
            };
        }




        private async Task OnFresh()
        {
            cfg = await goLogixService.GetJsConfig();

            var f = cfg.Functions.FirstOrDefault();
            if (f != null)
            {
                _curFunc = f;
            }

            _editor.SetValue(_curFunc.FunctionBody);
        }


        protected override async Task OnInitializedAsync()
        {
            // return base.OnInitializedAsync();


            cfg = await goLogixService.GetJsConfig();


            if (string.IsNullOrWhiteSpace(Id))
            {

            }
            else
            {


                var f = cfg.Functions.Where(fc => fc.FunctionName == Id).FirstOrDefault();
                if (f != null)
                {
                    _curFunc = f;
                }
            }

        }



        private async Task OnSave()
        {
            var code = await _editor.GetValue();

            _curFunc.FunctionBody = code;

            var fff = cfg.Functions.Where(c => c.Cmd == _curFunc.Cmd).FirstOrDefault();
            if (fff != null)
            {
                //fff.Cmd = _modle.Cmd;
                fff.FunctionBody = _curFunc.FunctionBody;
                fff.FunctionName = _curFunc.FunctionName;
                fff.Arguments = _curFunc.Arguments;
                fff.Returns = _curFunc.Returns;
                fff.Description = _curFunc.Description;

            }
            else
            {
                cfg.Functions.Add(_curFunc);
            }


            var b = await goLogixService.SubmitJsConfig(cfg);

            if (b)
            {
                //await message.InfoAsync("Save Ok");
                OnClose();
            }
            else
            {
                await message.ErrorAsync("Save Error");
            }




        }



        Models.JsFunc.JsFuncDef _curFunc = new Models.JsFunc.JsFuncDef();




        bool _visible = false;
        bool _confirmLoading = false;
        ArgumentDef _tmpArg = new ArgumentDef();
        string _title = string.Empty;

        bool _isAdd, _isInput;
        int _argIndex;

        private async Task AddOrUpdateArgument(bool is_add,bool is_input,int index)
        {
            
            _title = is_input ? "Input":"Return";
            _isAdd = is_add;
            _isInput = is_input;
            _argIndex = index;
            if (is_add)
            {
                _tmpArg = new ArgumentDef();
            }
            else
            {
                if (is_input)
                {
                    var arg = this._curFunc.Arguments.Where(c => c.Index == index).FirstOrDefault();

                    if(arg == null)
                    {
                        _isAdd = false;
                        _tmpArg = new ArgumentDef();
                    }
                    else
                    {
                        _tmpArg = new ArgumentDef();
                        _tmpArg.Index = arg.Index;
                        _tmpArg.Name = arg.Name;
                        _tmpArg.DataType = arg.DataType;
                    }
                }
                else
                {
                    var arg = this._curFunc.Returns.Where(c => c.Index == index ).FirstOrDefault();

                    if (arg == null)
                    {
                        _isAdd = false;
                        _tmpArg = new ArgumentDef();
                    }
                    else
                    {

                        _tmpArg = new ArgumentDef();
                        _tmpArg.Index = arg.Index;
                        _tmpArg.Name = arg.Name;
                        _tmpArg.DataType = arg.DataType;
                    }
                }
            }


            _visible = true;

        }

        private void OnHandleOk()
        {
            if (_isAdd)
            {
                if (_isInput)
                {
                    AddArgument_Input();
                }
                else
                {
                    AddArgument_Return();
                }
            }
            else
            {
                if (_isInput)
                {
                    EditArgument_Input(_argIndex);
                }
                else
                {
                    EditArgument_Return(_argIndex);
                }
            }

            _visible = false;
            StateHasChanged();

        }


        private void OnHandleCancel()
        {
            _visible = false;
        }

        private async Task AddArgument_Input()
        {
            if(this._curFunc.Arguments.Where(c=>c.Index == _tmpArg.Index && c.Name == _tmpArg.Name).Count() > 0)
            {
                await message.ErrorAsync("Index And Name already exists");
                return;
            }

            _curFunc.Arguments.Add(_tmpArg);

        }
        private async Task EditArgument_Input(int index)
        {
            var cc = _curFunc.Arguments.Where(c => c.Index == index).FirstOrDefault();

            if(cc == null)
            {
                return;
            }
            else
            {
                cc.Index = _tmpArg.Index;
                cc.Name = _tmpArg.Name;
                cc.DataType = _tmpArg.DataType;
            }

        }

        private async Task AddArgument_Return()
        {
            if (this._curFunc.Returns.Where(c => c.Index == _tmpArg.Index && c.Name == _tmpArg.Name).Count() > 0)
            {
                await message.ErrorAsync("Index And Name already exists");
                return;
            }

            _curFunc.Returns.Add(_tmpArg);
        }
        private async Task EditArgument_Return(int index)
        {
            var cc = _curFunc.Returns.Where(c => c.Index == index).FirstOrDefault();

            if (cc == null)
            {
                return;
            }
            else
            {
                cc.Index = _tmpArg.Index;
                cc.Name = _tmpArg.Name;
                cc.DataType = _tmpArg.DataType;
            }
        }

        private async Task DelArgument_Input(int index)
        {
            _curFunc.Arguments.RemoveAll(c => c.Index == index);
            StateHasChanged();
        }

        private async Task DelArgument_Return(int index)
        {

            _curFunc.Returns.RemoveAll(c => c.Index == index);
            StateHasChanged();
        }


        class FormItemLayout
        {
            public ColLayoutParam LabelCol { get; set; }
            public ColLayoutParam WrapperCol { get; set; }
        }

        private readonly FormItemLayout _formItemLayout = new FormItemLayout
        {
            LabelCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24 },
                Sm = new EmbeddedProperty { Span = 7 },
            },

            WrapperCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24 },
                Sm = new EmbeddedProperty { Span = 12 },
                Md = new EmbeddedProperty { Span = 10 },
            }
        };

        private readonly FormItemLayout _submitFormLayout = new FormItemLayout
        {
            WrapperCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24, Offset = 0 },
                Sm = new EmbeddedProperty { Span = 10, Offset = 7 },
            }
        };

    


        void UpdateTab()
        {
            ReuseTabsService.Update();
        }


        void OnClose()
        {
            this.ReuseTabsService.CloseCurrent();
        }
    }
}
