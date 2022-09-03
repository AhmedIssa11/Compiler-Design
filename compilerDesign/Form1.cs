using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace compilerDesign
{
	public partial class Form1 : Form
	{
		public Form1() {
			InitializeComponent();
		}
		List<string> varsize = new List<string>();
		void button1_Click( object sender, EventArgs e ) {
			console.Text = "";
			string sourceCode = textEditor.Text;

			var tokens = Scanner( Splitter( sourceCode ) );
			foreach ( var s in tokens ) {
				console.Text += s.name + ", " + s.type + "\r\n";
			}
			console.Text += "\r\n";

            var errors = SemanticAnalyzer( tokens );
			foreach ( var s in errors ) {
				error.Text += s + "\r\n";
			}

			for (int i = 0; i < Lvar.Count(); i++)
			{
                if (Lvar[i].integer != 0)
                {
                    string[] row = { Lvar[i].name, Convert.ToString(Lvar[i].integer.GetType()), Convert.ToString(Lvar[i].integer) };

                    memoryGrid.Rows.Add(row);
                }

			}


		}

		List<string> Splitter( string sourceCode ) {
			List<string> splitSourceCode = new List<string>();
			Regex RE = new Regex( @"\d+\.\d+|\'.*\'|\w+|\(|\)|\++|-+|\*|%|,|;|&+|\|+|<=|<|>=|>|==|=|!=|!|\{|\}|\/" );
			foreach ( Match m in RE.Matches( sourceCode ) ) {
				splitSourceCode.Add( m.Value );
			}
			return splitSourceCode;
		}

		class Token
		{
			public string name = "";
			public string type = "";

			public Token() {
			}

			public Token( string name, string type ) {
				this.name = name;
				this.type = type;
			}
		}
		class variable
        {
			public string name;
			public int integer;
			public float floaat;
			public string str;
			public double doubl;
			public bool bol;
			public char charachter;



        }
		int f = 0;
		int ct = 0;
		int fstop = 0;
		string prevvar;
		string prevop;
		string firstvar;
		int flagoutput = 0;
		int number = 0;
		int number2 = 0;
		string prevtype;
		string prevtypevar;
		string vartype;
		List<variable> Lvar = new List<variable>();
		List<Token> Scanner( List<string> splitCode ) {
			List<Token> output = new List<Token>();
			List<string> identifiers = new List<string>( new string[] { "int", "float", "string", "double", "bool", "char" } );
			List<string> symbols = new List<string>( new string[] { "+", "-", "/", "%", "*", "(", ")", "{", "}", ",", ";", "&&",
																		"||", "<", ">", "=", "!","++","==",">=","<=","!=" } );
			List<string> reservedWords = new List<string>( new string[] { "for", "while", "if", "do", "return", "break", "continue", "end" } );
			bool match = false;
		

			for ( int i = 0; i < splitCode.Count; i++ ) {
				if ( identifiers.Contains( splitCode[i] ) && match == false ) {
					output.Add( new Token( splitCode[i], "identifier" ) );
					match = true;
				}
				if ( symbols.Contains( splitCode[i] ) && match == false ) {
					output.Add( new Token( splitCode[i], "symbol" ) );
					match = true;
				}
				if ( reservedWords.Contains( splitCode[i] ) && match == false ) {
					output.Add( new Token( splitCode[i], "reserved word" ) );
					match = true;
				}
				if ( float.TryParse( splitCode[i], out _ ) && match == false ) {
					output.Add( new Token( splitCode[i], "number" ) );
					match = true;
				}
				if ( isValidVar( splitCode[i] ) && match == false ) {

					variable pnn = new variable();
					pnn.name = splitCode[i];
                    Lvar.Add(pnn);
					output.Add( new Token( splitCode[i], "variable" ) );
					match = true;
				}
				if ( splitCode[i].StartsWith( "'" ) && splitCode[i].EndsWith( "'" ) && match == false ) {
					output.Add( new Token( splitCode[i], "string" ) );
					match = true;
				}
				if ( match == false ) {
					output.Add( new Token( splitCode[i], "unknown" ) );
				}
				match = false;
			}
			return output;

			bool isValidVar( string v ) {
				if ( v.Length >= 1 ) {
					if ( char.IsLetter( v[0] ) || v[0] == '_' ) {
						return true;
					}
					else {
						return false;
					}
				}
				else {
					return false;
				}
			}
		}

		List<string> SemanticAnalyzer( List<Token> tokens ) {
			List<string> errors = new List<string>();
			Token prevInput1 = new Token();
			Token prevInput2 = new Token();
			Token prevInput3 = new Token();

			int selectedRule = 0;
			for ( int i = 0; i < tokens.Count; i++ ) {
				if ( selectedRule == 0 ) {
					if ( Rule1( tokens[i] ).StartsWith( "Start" ) ) {
						selectedRule = 1;
						continue;
					}
					if ( Rule2( tokens[i] ).StartsWith( "Start" ) ) {
						selectedRule = 2;
						continue;
					}
					if ( Rule3( tokens[i] ).StartsWith( "Start" ) ) {
						selectedRule = 3;
						continue;
					}
				}

				if ( selectedRule == 1 ) {
					var state = Rule1( tokens[i] );
					if ( state.StartsWith( "Ok" ) || state.StartsWith( "Error" ) ) {
						errors.Add( state );
						selectedRule = 0;
					}
				}
				if ( selectedRule == 2 ) {
					var state = Rule2( tokens[i] );
					if ( state.StartsWith( "Ok" ) || state.StartsWith( "Error" ) ) {
						errors.Add( state );
						selectedRule = 0;
					}
				}
				if ( selectedRule == 3 ) {
					var state = Rule3( tokens[i] );
					if ( state.StartsWith( "Ok" ) || state.StartsWith( "Error" ) ) {
						errors.Add( state );
						selectedRule = 0;
					}
				}
			}

			if ( selectedRule == 1 ) {
				errors.Add( Rule1( new Token() ) );
			}
			if ( selectedRule == 2 ) {
				errors.Add( Rule2( new Token() ) );
			}
			if ( selectedRule == 3 ) {
				errors.Add( Rule3( new Token() ) );
			}

			string Rule1( Token input ) {
				List<string> identifiers = new List<string>(new string[] { "int", "float", "string", "double", "bool", "char" });
				if ( prevInput1.name == "" && input.type == "identifier" ) {
					prevInput1 = input;
					if(prevInput1.name=="int")
                    {
						f = 1;
                    }
					if (prevInput1.name == "float")
					{
						f = 2;
					}
					if (prevInput1.name == "string")
					{
						f = 3;
					}
					if (prevInput1.name == "double")
					{
						f = 4;
					}
					if (prevInput1.name == "bool")
					{
						f = 5;
					}
					if (prevInput1.name == "char")
					{
						f = 6;
					}
					return "Start Rule 1";
				}
				else if ( prevInput1.type == "identifier" ) {
					string state = Rule2( input );
					if ( state.StartsWith( "Ok" ) ) {
						prevInput1 = new Token();
					}
					if ( state != "Error Rule 2" ) {
						return state.Substring( 0, state.IndexOf( "Rule 2" ) - 1 ) + " Rule 1";
					}
				}
				if ( prevInput1.type == "identifier" ) {
					prevInput1 = new Token();
					return "Error Expected 'variable' Rule 1";
				}
				prevInput1 = new Token();
				return "Error Rule 1";
			}

			string Rule2( Token input ) {
				List<string> operators = new List<string>( new string[] { "+", "-", "/", "%", "*" } );
				List<Int32> memoryint = new List<Int32>();
			

				if ( prevInput2.name == "" && input.type == "variable" ) {
					prevInput2 = input;
					
					return "Start Rule 2";
				}
				else if ( prevInput2.type == "variable" && input.name == ";" ) {
					prevInput2 = new Token();
					fstop = 1;
					return "Ok Rule 2";
				}
				else if ( prevInput2.type == "variable" && input.name == "=" ) {
					firstvar = prevInput2.name;
					prevvar = prevInput2.name;
					prevInput2 = input;
					
					return "Continue Rule 2";
				}
				else if ( prevInput2.name == "=" && input.type == "variable" ) {
					prevInput2 = input;
					prevtypevar = input.name;
					if (f == 1)
					{
						
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == prevInput2.name && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == prevvar)
										{
										 if (fstop == 1)
										 {
											Lvar[j].integer = Lvar[i].integer;
											varsize.Add(Convert.ToString(Lvar[j].integer));
										
											flagoutput = 1;
											break;
										 } 
									    }

									}


								}

							}
						


					}
				

					return "Continue Rule 2";
				}
				else if ( prevInput2.name == "=" && input.type == "number" ) {
					prevInput2 = input;
					if (f == 1)
					{ //
						   varsize.Add(prevInput2.name);
						   Lvar[ct].integer = Convert.ToInt32(prevInput2.name);
					
					}
			

					return "Continue Rule 2";
				}
				else if ( prevInput2.type == "number" && input.name == ";" ) {
					prevInput2 = new Token();
					ct++;
					return "Ok Rule 2";
				}
				else if ( prevInput2.type == "number" && operators.Contains( input.name ) ) {
					if (f == 1)
					{ //
						number = Convert.ToInt32(prevInput2.name);
						number2 = Convert.ToInt32(prevInput2.name);
						prevtype = prevInput2.type;
						
					}
					prevInput2 = input;

					return "Continue Rule 2";
				}
				else if ( prevInput2.type == "variable" && operators.Contains( input.name ) ) {

					prevvar = prevInput2.name;
					vartype = prevInput2.type;
					fstop = 1;
					prevInput2 = input;
					return "Continue Rule 2";
				}
				else if ( operators.Contains( prevInput2.name ) && input.type == "number" ) {
					if(prevInput2.name=="+")
                    { //
					

						if (f == 1 && prevtype == "number" )
						{ //
							
							Lvar[ct].integer = number + Convert.ToInt32(input.name);
							varsize.Add(Convert.ToString(Lvar[ct].integer));
							number2 = Convert.ToInt32(input.name);
							
						}
						if (f == 1&& vartype=="variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == prevtypevar && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											
											Lvar[j].integer = Lvar[i].integer + Convert.ToInt32(input.name);
											varsize.Add(Convert.ToString(Lvar[j].integer));
											
											flagoutput = 1;
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}


								}

							}

						}





					}
					if (prevInput2.name == "-")
					{


						if (f == 1&&prevtype == "number")
						{
							
							Lvar[ct].integer = number - Convert.ToInt32(input.name);
							varsize.Add(Convert.ToString(Lvar[ct].integer));
							



						}

						if (f == 1 && vartype == "variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == prevtypevar && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											
											Lvar[j].integer = Lvar[i].integer - Convert.ToInt32(input.name);
											varsize.Add(Convert.ToString(Lvar[j].integer));
											
											flagoutput = 1;
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}


								}

							}

						}


					}
					if (prevInput2.name == "*" )
					{


						


							if (f == 1 && prevtype == "number")
							{

								Lvar[ct].integer = number * Convert.ToInt32(input.name);
								varsize.Add(Convert.ToString(Lvar[ct].integer));
								number2 = Convert.ToInt32(input.name);
							
							}
							if (f == 1 && vartype == "variable")
				        {
								flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == prevtypevar && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{

											Lvar[j].integer = Lvar[i].integer * Convert.ToInt32(input.name);
											varsize.Add(Convert.ToString(Lvar[j].integer));
											
											flagoutput = 1;
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}


								}
							}
					     }

							
					}
					if (prevInput2.name == "/")
					{


						if (f == 1 && prevtype == "number")
						{
							
							Lvar[ct].integer = number / Convert.ToInt32(input.name);
							varsize.Add(Convert.ToString(Lvar[ct].integer));
						



						}
						if (f == 1 && vartype == "variable")
						{

							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == prevtypevar && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											
											Lvar[j].integer = Lvar[i].integer / Convert.ToInt32(input.name);
											varsize.Add(Convert.ToString(Lvar[j].integer));
											
											flagoutput = 1;
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}


								}

							}
						}

					}
					if (prevInput2.name == "%")
					{


						if (f == 1 && prevtype == "number")
						{
							
							Lvar[ct].integer = number % Convert.ToInt32(input.name);
							varsize.Add(Convert.ToString(Lvar[ct].integer));
							



						}
						if (f == 1 && vartype == "variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == prevtypevar && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											
											Lvar[j].integer = Lvar[i].integer % Convert.ToInt32(input.name);
											varsize.Add(Convert.ToString(Lvar[j].integer));
										
											flagoutput = 1;
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}


								}

							}
						}

					}

					prevInput2 = input;
					
					
					


					return "Continue Rule 2";
				}
				else if ( operators.Contains( prevInput2.name ) && input.type == "variable" ) {

					if (prevInput2.name == "+")
					{


						fstop = 0;

						if (f==1 && prevtype=="number")
                        {
							

							for (int i = 0; i < Lvar.Count; i++)
							{  if (Lvar[i].name == input.name)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											if (fstop == 0)
											{
												Lvar[j].integer = number2 + Lvar[i].integer;

												
												flagoutput = 1;
												break;
											}
										}
									}
									if(flagoutput==1)
                                    {
										break;
                                    }
								}
							}

						}
						
						if (f == 1 && vartype=="variable")
						{
							flagoutput = 0;


							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == prevvar)
										{  for (int k = 0; k < Lvar.Count; k++)
											{
												if (firstvar == Lvar[k].name)
												{
													
													Lvar[k].integer = Lvar[j].integer + Lvar[i].integer;
													varsize.Add(Convert.ToString(Lvar[k].integer));
													
													flagoutput = 1;
													break;
												}
											}
										}
										if(flagoutput==1)
                                        {
											break;
                                        }

									}
									if (flagoutput == 1)
									{
										break;
									}

								}

							}
							
						}
						

					}
					if (prevInput2.name == "-")
					{
						fstop = 0;

						if (f == 1 && prevtype == "number")
						{
							

							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name )
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											if (fstop == 0)
											{
												Lvar[j].integer = number2 - Lvar[i].integer;

												
												flagoutput = 1;
												break;
											}
										}
									}
									if (flagoutput == 1)
									{
										break;
									}
								}
							}

						}


						if (f == 1&& vartype=="variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == prevvar)
										{
											for (int k = 0; k < Lvar.Count; k++)
											{
												if (firstvar == Lvar[k].name)
												{
													
													Lvar[k].integer = Lvar[j].integer - Lvar[i].integer;
													varsize.Add(Convert.ToString(Lvar[k].integer));
													
													flagoutput = 1;
													break;
												}
											}
										}
										if (flagoutput == 1)
										{
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}


								}

							}
						}


					}
					if (prevInput2.name == "*")
					{
						fstop = 0;

						if (f == 1 && prevtype == "number")
						{
							

							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name )
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											if (fstop == 0)
											{
												Lvar[j].integer = number2 * Lvar[i].integer;

												
												flagoutput = 1;
												break;
											}
										}
									}
									if (flagoutput == 1)
									{
										break;
									}
								}
							}

						}

						if (f == 1 && vartype=="variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == prevvar)
										{
											for (int k = 0; k < Lvar.Count; k++)
											{
												if (firstvar == Lvar[k].name)
												{

													Lvar[k].integer = Lvar[j].integer * Lvar[i].integer;
													varsize.Add(Convert.ToString(Lvar[k].integer));
												
													flagoutput = 1;
													break;
												}
											}
										}
										if (flagoutput == 1)
										{
											break;
										}

									}

									if (flagoutput == 1)
									{
										break;
									}
								}

							}
						}


					}
					if (prevInput2.name == "/")
					{
						fstop = 0;

						if (f == 1 && prevtype == "number")
						{
							

							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name )
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											if (fstop == 0)
											{
												Lvar[j].integer = number2 /Lvar[i].integer;

											
												flagoutput = 1;
												break;
											}
										}
									}
									if (flagoutput == 1)
									{
										break;
									}
								}
							}

						}

						if (f == 1 && vartype=="variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == prevvar)
										{
											for (int k = 0; k < Lvar.Count; k++)
											{
												if (firstvar == Lvar[k].name)
												{

													Lvar[k].integer = Lvar[j].integer / Lvar[i].integer;
													varsize.Add(Convert.ToString(Lvar[k].integer));
												
													flagoutput = 1;
													break;
												}
											}
										}
										if (flagoutput == 1)
										{
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}

								}

							}
						}


					}
					if (prevInput2.name == "%")
					{
						fstop = 0;

						if (f == 1 && prevtype == "number")
						{
							

							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name )
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == firstvar)
										{
											if (fstop == 0)
											{
												Lvar[j].integer = number2 % Lvar[i].integer;

												flagoutput = 1;
												break;
											}
										}
									}
									if (flagoutput == 1)
									{
										break;
									}
								}
							}

						}

						if (f == 1 && vartype=="variable")
						{
							flagoutput = 0;
							for (int i = 0; i < Lvar.Count; i++)
							{
								if (Lvar[i].name == input.name && flagoutput == 0)
								{
									for (int j = 0; j < Lvar.Count; j++)
									{
										if (Lvar[j].name == prevvar)
										{
											for (int k = 0; k < Lvar.Count; k++)
											{
												if (firstvar == Lvar[k].name)
												{

													Lvar[k].integer = Lvar[j].integer % Lvar[i].integer;
													varsize.Add(Convert.ToString(Lvar[k].integer));
													
													flagoutput = 1;
													break;
												}
											}
										}
										if (flagoutput == 1)
										{
											break;
										}

									}
									if (flagoutput == 1)
									{
										break;
									}

								}

							}
						}


					}

					prevInput2 = input;

					return "Continue Rule 2";
				}

				if ( prevInput2.type == "variable" ) {
					prevInput2 = new Token();
					return "Error Expected ';' Or '=' Rule 2";
				}
				if ( prevInput2.name == "=" ) {
					prevInput2 = new Token();
					return "Error Expected 'number' Or 'variable' Rule 2";
				}
				if ( prevInput2.type == "variable" ) {
					prevInput2 = new Token();
					return "Error Expected ';' Or 'operator' Rule 2";
				}
				if ( prevInput2.type == "number" ) {
					prevInput2 = new Token();
					return "Error Expected ';' Or 'operator' Rule 2";
				}
				if ( operators.Contains( prevInput2.name ) ) {
					prevInput2 = new Token();
					return "Error Expected 'number' Or 'variable' Rule 2";
				}
				prevInput2 = new Token();
				return "Error Rule 2";
				
			}

			string Rule3( Token input ) {
				List<string> comp_operators = new List<string>( new string[] { "==", "!=", "<=", "<", ">", ">=" } );
				List<string> bool_operators = new List<string>( new string[] { "&&", "||" } );
				if ( prevInput3.name == "" && input.name == "if" ) {
					prevInput3 = input;
					return "Start Rule 3";
				}
				else if ( prevInput3.name == "if" && input.name == "(" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.name == "(" && input.type == "variable" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.type == "variable" && comp_operators.Contains( input.name ) ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( comp_operators.Contains( prevInput3.name ) && input.type == "number" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( comp_operators.Contains( prevInput3.name ) && input.type == "variable" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.type == "number" && bool_operators.Contains( input.name ) ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.type == "variable" && bool_operators.Contains( input.name ) ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( bool_operators.Contains( prevInput3.name ) && input.type == "variable" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.type == "number" && input.name == ")" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.type == "variable" && input.name == ")" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.name == ")" && input.name == "{" ) {
					prevInput3 = input;
					return "Continue Rule 3";
				}
				else if ( prevInput3.name == "{" && input.name == "}" ) {
					prevInput3 = new Token();
					return "Ok Rule 3";
				}
				else if ( prevInput3.name == "{" && input.name != "" ) {
					return "Continue Rule 3";
				}

				if ( prevInput3.name == "if" ) {
					prevInput3 = new Token();
					return "Error Expected '(' Rule 3";
				}
				if ( prevInput3.name == "(" ) {
					prevInput3 = new Token();
					return "Error Expected 'variable' Rule 3";
				}
				if ( prevInput3.type == "variable" ) {
					prevInput3 = new Token();
					return "Error Expected 'comp_operator' Or 'bool_operator' Or ')' Rule 3";
				}
				if ( comp_operators.Contains( prevInput3.name ) ) {
					prevInput3 = new Token();
					return "Error Expected 'number' Or 'variable' Rule 3";
				}
				if ( bool_operators.Contains( prevInput3.name ) ) {
					prevInput3 = new Token();
					return "Error Expected 'variable' Rule 3";
				}
				if ( prevInput3.type == "number" ) {
					prevInput3 = new Token();
					return "Error Expected 'bool_operator' Or ')' Rule 3";
				}
				if ( prevInput3.name == ")" ) {
					prevInput3 = new Token();
					return "Error Expected '{' Rule 3";
				}
				if ( prevInput3.name == "{" ) {
					prevInput3 = new Token();
					return "Error Expected '}' Rule 3";
				}

				prevInput3 = new Token();
				return "Error Rule 3";
			}
			return errors;
		}

        private void label3_Click(object sender, EventArgs e)
        {
            console.BringToFront();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            error.BringToFront();
        }
    }
}
