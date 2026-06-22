//-----------------------------------------------------------------------
// <copyright file="ForgeSchemaHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Helper class for TreeWalkerUnitTests that holds ForgeSchema examples.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Forge.TreeWalker.UnitTests
{
    public static class ForgeSchemaHelper
    {
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

        public const string ActionException_ContinuationOnRetryExhaustion_And_Skip = @"
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
                        },
                        ""ChildSelector"":
                        [
                            {
                                ""Label"": ""WhenActionSkip_IsSkipped"",
                                ""ShouldSelect"": ""C#|Session.GetCurrentNodeSkipActionContext() == \""Skipped\"""",
                                ""Child"": ""TestDelayExceptionAction_TreeNode""
                            },
                            {
                                ""Label"": ""WhenActionSkip_IsNotSkipped_ButNotEmpty"",
                                ""ShouldSelect"": ""C#|!string.IsNullOrWhiteSpace(Session.GetCurrentNodeSkipActionContext()) && Session.GetCurrentNodeSkipActionContext() != \""Skipped\"""",
                                ""Child"": ""ReturnSessionIdAction""
                            }
                        ]
                    },
                    ""TestDelayExceptionAction_TreeNode"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""TestDelayExceptionAction_0"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""ThrowException"": false
                                }
                            }
                        }
                    },
                    ""ReturnSessionIdAction"": {
                        ""Type"": ""Action"",
                            ""Actions"": {
                            ""ReturnSessionIdAction_0"": {
                                ""Action"": ""ReturnSessionIdAction""
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

        public const string ActionDelay_ContinuationOnRetryExhaustion_RetryPolicy_FixedCount = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""ThrowException"": true,
                                    ""DelayMilliseconds"": 10
                                },
                                ""Timeout"": 100,
                                ""RetryPolicy"": {
                                    ""Type"": ""FixedCount"",
                                    ""MinBackoffMs"": 25,
                                    ""MaxRetry"": 2,
                                },
                                ""ContinuationOnTimeout"": true,
                                ""ContinuationOnRetryExhaustion"": true
                            }
                        }
                    }
                }
            }";

        public const string ActionDelay_ContinuationOnTimeout_RetryPolicy_FixedCount = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestDelayExceptionAction"": {
                                ""Action"": ""TestDelayExceptionAction"",
                                ""Input"": {
                                    ""ThrowException"": true,
                                    ""DelayMilliseconds"": 150
                                },
                                ""Timeout"": 100,
                                ""RetryPolicy"": {
                                    ""Type"": ""FixedCount"",
                                    ""MinBackoffMs"": 25,
                                    ""MaxRetryCount"": 2,
                                },
                                ""ContinuationOnTimeout"": true,
                                ""ContinuationOnRetryExhaustion"": true
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

        public const string ChildSelectorLiteralChild = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Literal child"",
                                ""Child"": ""LiteralLeaf""
                            }
                        ]
                    },
                    ""LiteralLeaf"": {
                        ""Type"": ""Leaf""
                    }
                }
            }
        ";

        public const string ChildSelectorDynamicChild = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TardigradeAction"": {
                                ""Action"": ""TardigradeAction""
                            }
                        },
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Dynamic child"",
                                ""ShouldSelect"": ""C#|Session.GetLastActionResponse().Status == \""Success\"""",
                                ""Child"": ""C#|UserContext.ResourceType == \""Container\"" ? \""DynamicLeaf\"" : \""UnexpectedLeaf\""""
                            }
                        ]
                    },
                    ""DynamicLeaf"": {
                        ""Type"": ""Leaf""
                    },
                    ""UnexpectedLeaf"": {
                        ""Type"": ""Leaf""
                    }
                }
            }
        ";

        public const string ChildSelectorDynamicChildInvalidType = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""ChildSelector"": [
                            {
                                ""Label"": ""Invalid dynamic child"",
                                ""Child"": ""C#|1""
                            }
                        ]
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

        public const string TestEvaluateInputTypeAction_UndefinedEnumMemberFail = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Action"",
                        ""Actions"": {
                            ""Root_TestEvaluateInputTypeAction"": {
                                ""Action"": ""TestEvaluateInputTypeAction"",
                                ""Input"": {
                                    ""FooEnumArray"": [
                                        ""UNDEFINED_Enum_Value""
                                    ]
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

        public const string ReExecuteNodeSchema = @"
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
                                ""ShouldSelect"": ""C#|(int)Session.GetLastActionResponse().Output > 2"",
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

        #region CacheVars Schemas

        /// <summary>
        /// Basic CacheVars test — static Roslyn expression used in ShouldSelect.
        /// </summary>
        public const string CacheVars_StaticExpression = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""myVal"": ""C#|42""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|(int)Cache.myVal == 42"",
                                ""Child"": ""CorrectValue""
                            },
                            {
                                ""Child"": ""WrongValue""
                            }
                        ]
                    },
                    ""CorrectValue"": {
                        ""Type"": ""Leaf""
                    },
                    ""WrongValue"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars referencing Session.GetOutput to extract ActionResponse data.
        /// </summary>
        public const string CacheVars_SessionGetOutput = @"
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
                        ""CacheVars"": {
                            ""actionStatus"": ""C#|Session.GetOutput(\""Root_CollectDiagnosticsAction\"").Status""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.actionStatus == \""Success\"""",
                                ""Child"": ""Found""
                            },
                            {
                                ""Child"": ""NotFound""
                            }
                        ]
                    },
                    ""Found"": {
                        ""Type"": ""Leaf""
                    },
                    ""NotFound"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars using UserContext.
        /// </summary>
        public const string CacheVars_UserContext = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""userName"": ""C#|UserContext.Name""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.userName == \""MyName\"""",
                                ""Child"": ""Found""
                            },
                            {
                                ""Child"": ""NotFound""
                            }
                        ]
                    },
                    ""Found"": {
                        ""Type"": ""Leaf""
                    },
                    ""NotFound"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars are node-scoped — second node should NOT see first node's cache variables.
        /// </summary>
        public const string CacheVars_NodeScoped = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""firstNodeVar"": ""C#|99""
                        },
                        ""ChildSelector"": [
                            {
                                ""Child"": ""SecondNode""
                            }
                        ]
                    },
                    ""SecondNode"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""secondNodeVar"": ""C#|\""present\""""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.secondNodeVar == \""present\"" && Cache.firstNodeVar == null"",
                                ""Child"": ""Isolated""
                            },
                            {
                                ""Child"": ""Leaked""
                            }
                        ]
                    },
                    ""Isolated"": {
                        ""Type"": ""Leaf""
                    },
                    ""Leaked"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars with invalid expression — should throw EvaluateDynamicPropertyException.
        /// </summary>
        public const string CacheVars_InvalidExpression = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""badVar"": ""C#|NonExistentObject.Property""
                        },
                        ""ChildSelector"": [
                            {
                                ""Child"": ""End""
                            }
                        ]
                    },
                    ""End"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// Multiple CacheVars on the same node.
        /// </summary>
        public const string CacheVars_Multiple = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""greeting"": ""C#|\""hello\"""",
                            ""target"": ""C#|\""world\"""",
                            ""suffix"": ""C#|\""!\""""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.greeting == \""hello\"" && Cache.target == \""world\"" && Cache.suffix == \""!\"""",
                                ""Child"": ""Found""
                            },
                            {
                                ""Child"": ""NotFound""
                            }
                        ]
                    },
                    ""Found"": {
                        ""Type"": ""Leaf""
                    },
                    ""NotFound"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars with string literal (non-Roslyn value).
        /// </summary>
        public const string CacheVars_StringLiteral = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""literal"": ""hello world""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.literal == \""hello world\"""",
                                ""Child"": ""Found""
                            },
                            {
                                ""Child"": ""NotFound""
                            }
                        ]
                    },
                    ""Found"": {
                        ""Type"": ""Leaf""
                    },
                    ""NotFound"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars with ActionResponse object value — access nested property via Cache.
        /// </summary>
        public const string CacheVars_ObjectValue = @"
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
                        ""CacheVars"": {
                            ""response"": ""C#|Session.GetOutput(\""Root_CollectDiagnosticsAction\"")""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.response.Status == \""Success\"" && Cache.response.Output == \""TheCommand_Results\"""",
                                ""Child"": ""Found""
                            },
                            {
                                ""Child"": ""NotFound""
                            }
                        ]
                    },
                    ""Found"": {
                        ""Type"": ""Leaf""
                    },
                    ""NotFound"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars with boolean expression — IsReady is evaluated from an action result, then used in ShouldSelect.
        /// </summary>
        public const string CacheVars_BooleanExpression = @"
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
                        ""CacheVars"": {
                            ""IsSuccess"": ""C#|Session.GetOutput(\""Root_CollectDiagnosticsAction\"").Status == \""Success\""""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|(bool)Cache.IsSuccess"",
                                ""Child"": ""Ready""
                            },
                            {
                                ""Child"": ""NotReady""
                            }
                        ]
                    },
                    ""Ready"": {
                        ""Type"": ""Leaf""
                    },
                    ""NotReady"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// CacheVars on multiple node types — Selection, Action, and Subroutine.
        /// Multi-tree dictionary schema (RootTree + SubroutineTree); initialize via TestSubroutineInitialize.
        /// </summary>
        public const string CacheVars_AllNodeTypes = @"
            {
                ""RootTree"": {
                    ""Tree"": {
                        ""Root"": {
                            ""Type"": ""Selection"",
                            ""CacheVars"": {
                                ""nodeType"": ""C#|\""selection\""""
                            },
                            ""ChildSelector"": [
                                {
                                    ""ShouldSelect"": ""C#|Cache.nodeType == \""selection\"""",
                                    ""Child"": ""ActionNode""
                                },
                                {
                                    ""Child"": ""Fail""
                                }
                            ]
                        },
                        ""ActionNode"": {
                            ""Type"": ""Action"",
                            ""Actions"": {
                                ""ActionNode_CollectDiagnosticsAction"": {
                                    ""Action"": ""CollectDiagnosticsAction"",
                                    ""Input"": {
                                        ""Command"": ""TheCommand""
                                    }
                                }
                            },
                            ""CacheVars"": {
                                ""nodeType"": ""C#|\""action\""""
                            },
                            ""ChildSelector"": [
                                {
                                    ""ShouldSelect"": ""C#|Cache.nodeType == \""action\"""",
                                    ""Child"": ""SubroutineNode""
                                },
                                {
                                    ""Child"": ""Fail""
                                }
                            ]
                        },
                        ""SubroutineNode"": {
                            ""Type"": ""Subroutine"",
                            ""Actions"": {
                                ""SubroutineNode_Subroutine"": {
                                    ""Action"": ""SubroutineAction"",
                                    ""Input"": {
                                        ""TreeName"": ""SubroutineTree"",
                                        ""TreeInput"": ""TestValue""
                                    }
                                }
                            },
                            ""CacheVars"": {
                                ""nodeType"": ""C#|\""subroutine\""""
                            },
                            ""ChildSelector"": [
                                {
                                    ""ShouldSelect"": ""C#|Cache.nodeType == \""subroutine\"""",
                                    ""Child"": ""End""
                                },
                                {
                                    ""Child"": ""Fail""
                                }
                            ]
                        },
                        ""End"": {
                            ""Type"": ""Leaf""
                        },
                        ""Fail"": {
                            ""Type"": ""Leaf""
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
            }";

        /// <summary>
        /// CacheVars same-named property across two nodes — confirms isolation (second node re-defines same var).
        /// </summary>
        public const string CacheVars_SameNameAcrossNodes = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""status"": ""C#|\""first\""""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.status == \""first\"""",
                                ""Child"": ""SecondNode""
                            },
                            {
                                ""Child"": ""Fail""
                            }
                        ]
                    },
                    ""SecondNode"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": {
                            ""status"": ""C#|\""second\""""
                        },
                        ""ChildSelector"": [
                            {
                                ""ShouldSelect"": ""C#|Cache.status == \""second\"""",
                                ""Child"": ""End""
                            },
                            {
                                ""Child"": ""Fail""
                            }
                        ]
                    },
                    ""End"": {
                        ""Type"": ""Leaf""
                    },
                    ""Fail"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        /// <summary>
        /// Invalid schema — CacheVars is a string instead of a dictionary/object.
        /// Expected to fail ForgeSchemaValidationRules (CacheVars must be an object).
        /// </summary>
        public const string CacheVars_InvalidType_NotADictionary = @"
            {
                ""Tree"": {
                    ""Root"": {
                        ""Type"": ""Selection"",
                        ""CacheVars"": ""UnexpectedStringNotADictionary"",
                        ""ChildSelector"": [
                            {
                                ""Child"": ""End""
                            }
                        ]
                    },
                    ""End"": {
                        ""Type"": ""Leaf""
                    }
                }
            }";

        #endregion CacheVars Schemas
    }
}
