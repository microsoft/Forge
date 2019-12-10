//-----------------------------------------------------------------------
// <copyright file="ForgeSchemaHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Helper class for TreeWalkerUnitTests that holds ForgeSchema examples.
// </summary>
//-----------------------------------------------------------------------

namespace Forge.TreeWalker.UnitTests
{
    public static class ForgeSchemaHelper
    {
        public const string TardigradeScenario = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Node"",
                                ""ShouldSelect"": ""C#|UserContext.ResourceType == \""Node\"""",
                                ""Child"": ""Node""
                            },
                            {
                                ""Label"": ""Label"",
                                ""ShouldSelect"": ""C#|UserContext.ResourceType == \""Container\"""",
                                ""Child"": ""Container""
                            }
                        ]
                    },
                    ""Container"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Container_CollectDiagnosticsAction"": {
                                ""Action"": ""CollectDiagnosticsAction"",
                                ""Input"":
                                {
                                    ""Command"": ""RunCollectDiagnostics.exe""
                                }
                            }
                        },
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Label"",
                                ""ShouldSelect"": ""C#|Session.GetLastActionResponse().Status == \""Success\"""",
                                ""Child"": ""Tardigrade""
                            }
                        ]
                    },
                    ""Tardigrade"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Tardigrade_TardigradeAction"": {
                                ""Action"": ""TardigradeAction""
                            }
                        },
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Label"",
                                ""ShouldSelect"": ""C#|Session.GetLastActionResponse().Status == \""Success\"""",
                                ""Child"": ""Tardigrade_Success""
                            }
                        ]
                    },
                    ""Tardigrade_Success"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        public const string ActionException_Fail = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""ThrowException"": true
                                }
                            }
                        }
                    }
                }
            }";

        public const string ActionException_ContinuationOnRetryExhaustion = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""ThrowException"": true
                                },
                                ""ContinuationOnRetryExhaustion"": true
                            }
                        }
                    }
                }
            }";

        public const string ActionDelay_Fail = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""DelayMilliseconds"": 50
                                },
                                ""Timeout"": 10,
                            }
                        }
                    }
                }
            }";

        public const string ActionDelay_ContinuationOnTimeout = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""DelayMilliseconds"": 50
                                },
                                ""Timeout"": 10,
                                ""ContinuationOnTimeout"": true
                            }
                        }
                    }
                }
            }";

        public const string ActionDelay_ContinuationOnTimeout_RetryPolicy_TimeoutInAction = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""DelayMilliseconds"": 50,
                                    ""ThrowException"": true
                                },
                                ""Timeout"": 100,
                                ""RetryPolicy"": {
                                    ""Type"": ""FixedInterval"",
                                    ""MinBackoffMs"": 25
                                },
                                ""ContinuationOnTimeout"": true
                            }
                        }
                    }
                }
            }";

        public const string ActionDelay_ContinuationOnTimeout_RetryPolicy_TimeoutBetweenRetries = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""DelayMilliseconds"": 25,
                                    ""ThrowException"": true
                                },
                                ""Timeout"": 50,
                                ""RetryPolicy"": {
                                    ""Type"": ""FixedInterval"",
                                    ""MinBackoffMs"": 100
                                },
                                ""ContinuationOnTimeout"": true
                            }
                        }
                    }
                }
            }";

        public const string NoChildMatch = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Label"",
                                ""ShouldSelect"": ""C#|false"",
                                ""Child"": ""LeafNode""
                            }
                        ]
                    },
                    ""LeafNode"": {
                        ""Type"": ""Leaf""
                    }
                }
            }
        ";

        public const string TestEvaluateInputTypeAction = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestEvaluateInputTypeAction"": {
                                ""Action"": ""TestEvaluateInputTypeAction"",
                                ""Input"": {
                                    ""Command"": ""tasklist"",
                                    ""IntExpression"": ""C#<Int64>|UserContext.GetCount()"",
                                    ""BoolExpression"": ""C#|true"",
                                    ""NestedObject"": {
                                        ""Name"": ""C#|string.Format(\""{0}_{1}\"", \""MyName\"", UserContext.Name)"",
                                        ""Value"": ""MyValue"",
                                        ""IntPropertyInObject"": ""C#<Int64>|UserContext.GetCount()""
                                    },
                                    ""ObjectArray"": [
                                        {
                                            ""Name"": ""C#|UserContext.Name"",
                                            ""Value"": ""FirstValue""
                                        },
                                        {
                                            ""Name"": ""SecondName"",
                                            ""Value"": ""SecondValue""
                                        }
                                    ],
                                    ""StringArray"": [
                                        ""C#|UserContext.Name"",
                                        ""value2""
                                    ],
                                    ""LongArray"": [
                                        ""C#<Int64>|(long)UserContext.GetCount()"",
                                        3,
                                        2
                                    ],
                                    ""BoolDelegate"": ""C#|(Func<bool>)(() => {return UserContext.GetCount() == 1;})"",
                                    ""BoolDelegateAsync"": ""C#|(Func<Task<bool>>)(async() => { return await UserContext.GetCountAsync() == 2; })"",
                                    ""StringDictionary"": {
                                        ""TestKey1"": ""C#|UserContext.Name"",
                                        ""TestKey2"": ""TestValue2""
                                    },
                                    ""DynamicObject"": {
                                        ""DynamicPropertyString"": ""TestValue1"",
                                        ""DynamicPropertyInt"": 10,
                                        ""DynamicPropertyExpression"": ""C#|UserContext.Name"",
                                        ""DynamicPropertyNestedObject"": {
                                            ""NestedPropertyOne"": ""NestedValue1""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        ";

        public const string TestEvaluateInputType_FailOnField_Action = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestEvaluateInputType_FailOnField_Action"": {
                                ""Action"": ""TestEvaluateInputType_FailOnField_Action"",
                                ""Input"": {
                                    ""UnexpectedField"": true
                                },
                                ""ContinuationOnRetryExhaustion"": true
                            }
                        }
                    }
                }
            }
        ";

        public const string TestEvaluateInputTypeAction_UnexpectedPropertyFail = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestEvaluateInputTypeAction"": {
                                ""Action"": ""TestEvaluateInputTypeAction"",
                                ""Input"": {
                                    ""UnexpectedProperty"": true
                                },
                                ""ContinuationOnRetryExhaustion"": true
                            }
                        }
                    }
                }
            }
        ";

        public const string TestEvaluateInputType_FailOnNonEmptyCtor_Action = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestEvaluateInputType_FailOnNonEmptyCtor_Action"": {
                                ""Action"": ""TestEvaluateInputType_FailOnNonEmptyCtor_Action"",
                                ""Input"": {
                                    ""BoolProperty"": true
                                },
                                ""ContinuationOnRetryExhaustion"": true
                            }
                        }
                    }
                }
            }
        ";

        public const string LeafNodeSummaryAction = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Leaf"",
                        ""Actions"": {
                            ""Root_LeafNodeSummaryAction"": {
                                ""Action"": ""LeafNodeSummaryAction"",
                                ""Input"": {
                                    ""Status"": ""Success"",
                                    ""StatusCode"": 1,
                                    ""Output"": ""TheResult""
                                }
                            }
                        }
                    }
                }
            }
        ";

        public const string LeafNodeSummaryAction_InputIsActionResponse = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_CollectDiagnosticsAction"": {
                                ""Action"": ""CollectDiagnosticsAction"",
                                ""Input"": {
                                    ""Command"": ""TheCommand""
                                }
                            }
                        },
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Label"",
                                ""Child"": ""LeafNodeSummaryTest""
                            }
                        ]
                    },
                    ""LeafNodeSummaryTest"": {
                        ""Type"": ""Leaf"",
                        ""Actions"": {
                            ""LeafNodeSummaryTest_LeafNodeSummaryAction"": {
                                ""Action"": ""LeafNodeSummaryAction"",
                                ""Input"": ""C#|Session.GetLastActionResponse()""
                            }
                        }
                    }
                }
            }
        ";

        public const string ExternalExecutors = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Leaf"",
                        ""Actions"": {
                            ""Root_LeafNodeSummaryAction"": {
                                ""Action"": ""LeafNodeSummaryAction"",
                                ""Input"": {
                                    ""Status"": ""External|StatusResult""
                                }
                            }
                        }
                    }
                }
            }
        ";

        public const string SubroutineAction_GetLastActionResponse = @"
            {
                ""ParentTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Subroutine"",
                            ""Actions"": {
                                ""Root_Subroutine"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree"",
                                        ""TreeInput"": {
                                            ""TestStatusCode"": 10
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                ""SubroutineTree"": {
                    ""RootTreeNodeKey"": ""StartingNode"",
                    ""Tree"": {
                        ""StartingNode"": {
                            ""Type"": ""Leaf"",
                            ""Actions"": {
                                ""Root_LeafNodeSummaryAction"": {
                                    ""Action"": ""LeafNodeSummaryAction"",
                                    ""Input"": {
                                        ""Status"": ""Success"",
                                        ""StatusCode"": ""C#|(int)TreeInput.TestStatusCode""
                                    }
                                }
                            }
                        }
                    }
                }
            }
        ";

        public const string SubroutineAction_NoActions = @"
            {
                ""RootTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Subroutine"",
                            ""Actions"": {
                                ""Root_Subroutine"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree"",
                                        ""TreeInput"": ""TestValue""
                                    }
                                }
                            }
                        }
                    }
                },
                ""SubroutineTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Leaf""
                        }
                    }
                }
            }
        ";

        public const string SubroutineAction_ParallelSubroutineActions = @"
            {
                ""RootTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Subroutine"",
                            ""Actions"": {
                                ""Root_Subroutine_One"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree"",
                                        ""TreeInput"": ""TestValueOne""
                                    }
                                },
                                ""Root_Subroutine_Two"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree"",
                                        ""TreeInput"": ""TestValueTwo""
                                    }
                                },
                                ""Root_CollectDiagnosticsAction"": {
                                    ""Action"": ""CollectDiagnosticsAction"",
                                    ""Input"": {
                                        ""Command"": ""TheCommand""
                                    }
                                }
                            }
                        }
                    }
                },
                ""SubroutineTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Leaf"",
                            ""Actions"": {
                                ""Root_LeafNodeSummaryAction"": {
                                    ""Action"": ""LeafNodeSummaryAction"",
                                    ""Input"": {
                                        ""Status"": ""C#|(string)TreeInput"",
                                    }
                                }
                            }
                        }
                    }
                }
            }
        ";

        public const string SubroutineAction_FailsOnActionTreeNodeType = @"
            {
                ""RootTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Action"",
                            ""Actions"": {
                                ""Root_Subroutine"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree"",
                                        ""TreeInput"": ""TestValue""
                                    }
                                }
                            }
                        }
                    }
                },
                ""SubroutineTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Leaf""
                        }
                    }
                }
            }
        ";

        public const string SubroutineAction_FailsOnNoSubroutineAction = @"
            {
                ""RootTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Subroutine""
                        }
                    }
                }
            }
        ";

        public const string CycleSchema = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_RevisitAction"": {
                                ""Action"": ""RevisitAction""
                            }
                        },
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Label"",
                                ""ShouldSelect"": ""C#|(int)Session.GetLastActionResponse().Output < 3"",
                                ""Child"": ""Root""
                            }
                        ]
                    }
                }
            }
        ";

        public const string Cycle_SubroutineActionUsesDifferentSessionId = @"
            {
                ""RootTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Subroutine"",
                            ""Actions"": {
                                ""Root_Subroutine"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree""
                                    }
                                },
                                ""Root_RevisitAction"": {
                                    ""Action"": ""RevisitAction""
                                }
                            },
                            ""ChildSelector"": [
                                {
                                    ""Label"": ""Label"",
                                    ""ShouldSelect"": ""C#|(int)Session.GetOutput(\""Root_RevisitAction\"").Output < 3"",
                                    ""Child"": ""Root""
                                }
                            ]
                        }
                    }
                },
                ""SubroutineTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Action"",
                                ""Actions"": {
                                ""Root_ReturnSessionIdAction"": {
                                    ""Action"": ""ReturnSessionIdAction""
                                }
                            }
                        }
                    }
                }
            }
        ";
    }
}