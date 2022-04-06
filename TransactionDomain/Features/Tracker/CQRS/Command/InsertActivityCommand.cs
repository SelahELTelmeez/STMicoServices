using MediatR;
using ResultHandler;
using TransactionDomain.Features.Activities.DTO.Command;

namespace TransactionDomain.Features.Activities.CQRS.Command;

public record InsertActivityCommand(InsertActivityRequest ActivityRequest) : IRequest<CommitResult<int>>;


//[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
//[WebMethod]
//public string LMS_LOActivityInsert(string UserId, int LessonId, int LOId, int Code, int Progress, double StudentPoints, int LearningDuration)
//{
//    string ReturnedResult = "Faild";
//    Error IsAjaxRequestResult = new Error();
//    IsAjaxRequestResult = IsAjaxRequest();
//    if (IsAjaxRequestResult.ErrorCode == 0)
//    {
//        LOUserActivity lo = new LOUserActivity();
//        lo.userId = UserId;
//        lo.lessonId = LessonId;
//        lo.LOId = LOId;
//        lo.code = Code;
//        lo.progress = Progress;
//        lo.Points = StudentPoints * Progress / 100;
//        lo.LearningDuration = LearningDuration;
//        lo.ActivityDateTime = System.DateTime.Now;
//        LMSdb.LOUserActivities.InsertOnSubmit(lo);
//        LMSdb.SubmitChanges();
//        int activityId = (from X in LMSdb.LOUserActivities
//                          where X.userId == UserId && X.LOId == LOId
//                          select X.id).Max();
//        lesson Lesson = (from X in LMSdb.lessons
//                            where X.id == LessonId
//                            select X).Single();
//        // // =========== Update Lesson Activity ================
//        var LessonActivity = (from X in LMSdb.LMSLessonActivities
//                              where X.UserId == UserId && X.LessonId == LessonId
//                              select X).ToList();
//        if (LessonActivity.Count() > 0)
//        {
//            string QueryUPdate = "update LMSLessonActivity set ";
//            QueryUPdate += "LessonPoints =" + Lesson.LessonPoints.ToString() + ", ";
//            QueryUPdate += "UserPoints =" + StudentPoints.ToString() + ", ";
//            QueryUPdate += "LastDateTime = getdate()";
//            QueryUPdate += " where LessonId=" + LessonId.ToString() + " And UserId = '" + UserId.ToString() + "'"; LMSdb.ExecuteCommand(QueryUPdate);
//        }
//        else
//        {
//            LMSLessonActivity newLessonActivity = new LMSLessonActivity();
//            newLessonActivity.LessonId = LessonId;
//            newLessonActivity.UserId = UserId;
//            newLessonActivity.LessonPoints = Lesson.LessonPoints;
//            newLessonActivity.UserPoints = StudentPoints;
//            newLessonActivity.UnitId = Lesson.unitid.Value;
//            newLessonActivity.SubjectId = Lesson.Unit.curricid;
//            newLessonActivity.LastDateTime = System.DateTime.Now; 
//            LMSdb.LMSLessonActivities.InsertOnSubmit(newLessonActivity);
//            LMSdb.SubmitChanges();
//        }
//        //====================================================
//        Error e = (from X in LMSdb.Errors
//                   where X.ErrorCode == 0
//                   select X).Single();
//        object obj = new object();
//        obj = new { ActivityId = activityId, Error = e }; 
//        ReturnedResult = JsonConvert.SerializeObject(obj);
//    }
//    else
//    {
//        ReturnedResult = JsonConvert.SerializeObject(IsAjaxRequestResult);
//    }
//    return ReturnedResult;
//} 
////================== Update LO Activity ==================

////[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
////[WebMethod]
//public string LMS_LOActivityUpdate(int ActivityId, string UserId, int LessonId, int Code, int Progress, double StudentPoints, int LearningDuration)
//{
//    /*This method is used to update the running user activity to track the user progress in the running LO*/
//    string ReturnedResult = "Faild";
//    Error IsAjaxRequestResult = new Error();
//    IsAjaxRequestResult = IsAjaxRequest();
//    if (IsAjaxRequestResult.ErrorCode == 0)
//    { //======================== Update LOUserActivity ===================================
//        string QueryCommand = "Update LOUserActivity set code =" + Code.ToString()
//        + ",progress =" + Progress.ToString() + ",Points=" + StudentPoints.ToString() + ",LearningDuration=" + LearningDuration.ToString()
//        + "where id=" + ActivityId.ToString();
//        LMSdb.ExecuteCommand(QueryCommand); 
//        //============================== modification =======================
//        lesson Lesson = (from X in LMSdb.lessons
//                         where X.id == LessonId
//                         select X).Single();
//        string SubjectId = Lesson.Unit.curricid;
//        int LevelBeforeActivity = 0; //======= Calculate the user level of this subject before this activity update ==================
//        var MedalLevel = (from X in LMSdb.LMSRewards
//                          where X.UserId == UserId && X.Grade == Lesson.Unit.curriculum.grade && X.Subject == Lesson.Unit.curriculum.shortname
//                          orderby X.MedalLevel descending
//                          select X.MedalLevel).ToList();
//        if (MedalLevel.Count() > 0)
//            LevelBeforeActivity = MedalLevel[0].Value;
//        else LevelBeforeActivity = 0;
//        // =========== Update Lesson Activity ================
//        var LessonActivity = (from X in LMSdb.LMSLessonActivities
//                              where X.UserId == UserId && X.LessonId == LessonId
//                              select X).ToList();
//        if (LessonActivity.Count() > 0) // case of exisiting record of lesson activity
//        {
//            LMSProgressModel progress = GetLessonProgress(UserId, LessonId);
//            string QueryUPdate = "update LMSLessonActivity set ";
//            QueryUPdate += "LessonPoints =" + Lesson.LessonPoints.ToString() + ", ";
//            QueryUPdate += "UserPoints =" + progress.UserPoints.ToString() + ", ";
//            QueryUPdate += "LastDateTime = getdate()";
//            QueryUPdate += " where LessonId=" + LessonId.ToString() + " And UserId = '" + UserId.ToString() + "'";
//            LMSdb.ExecuteCommand(QueryUPdate);
//        }
//        else
//        {
//            LMSLessonActivity newLessonActivity = new LMSLessonActivity();
//            newLessonActivity.LessonId = LessonId;
//            newLessonActivity.UserId = UserId;
//            newLessonActivity.LessonPoints = Lesson.LessonPoints;
//            newLessonActivity.UserPoints = StudentPoints;
//            newLessonActivity.UnitId = Lesson.unitid.Value;
//            newLessonActivity.SubjectId = SubjectId;
//            newLessonActivity.LastDateTime = System.DateTime.Now;
//            LMSdb.LMSLessonActivities.InsertOnSubmit(newLessonActivity);
//            try
//            {// case of there is no record of lesson activity
//                LMSdb.SubmitChanges();
//            }
//            catch (Exception)
//            { // case of exception due to the existance of lesson activity record
//                LMSProgressModel progress = GetLessonProgress(UserId, LessonId);
//                string QueryUPdate = "update LMSLessonActivity set ";
//                QueryUPdate += "LessonPoints =" + Lesson.LessonPoints.ToString() + ", ";
//                QueryUPdate += "UserPoints =" + progress.UserPoints.ToString() + ", ";
//                QueryUPdate += "LastDateTime = getdate()";
//                QueryUPdate += " where LessonId=" + LessonId.ToString() + " And UserId = '" + UserId.ToString() + "'";
//                LMSdb.ExecuteCommand(QueryUPdate);
//                throw;
//            }
//        }
//        var ProgressAfterActivity = LMSdb.LMSGetStudentSubjectProgress(UserId, Lesson.Unit.curricid).ToList().First();
//        int LevelAfterActivity = 0;
//        int progresslevel = Convert.ToInt32(ProgressAfterActivity.UserPoints.Value * 100 / ProgressAfterActivity.SubjectPoints.Value); if (progresslevel >= 20 && progresslevel < 40)
//            LevelAfterActivity = 1;
//        else if (progresslevel >= 40 && progresslevel < 60)
//            LevelAfterActivity = 2;
//        else if (progresslevel >= 60 && progresslevel < 80)
//            LevelAfterActivity = 3;
//        else if (progresslevel >= 80 && progresslevel < 95)
//            LevelAfterActivity = 4;
//        else if (progresslevel >= 95)
//            LevelAfterActivity = 5; if (LevelAfterActivity > LevelBeforeActivity)
//        {
//            LMSReward reward = new LMSReward();
//            reward.myDate = System.DateTime.Now;
//            reward.Type = 1;
//            reward.MedalLevel = LevelAfterActivity;
//            reward.Grade = Lesson.Unit.curriculum.grade;
//            reward.Term = Lesson.Unit.curriculum.term;
//            reward.Subject = Lesson.Unit.curriculum.shortname;
//            reward.UserId = UserId;
//            reward.IsNew = true;
//            reward.Image = Lesson.Unit.curricid.Substring(0, 4);
//            switch (LevelAfterActivity)
//            {
//                case 1:
//                    reward.Title = "ميدالية برنزية";
//                    reward.Description = "مبروك لقد حصلت على الميدالية البرنزية في مادة " + reward.Subject;
//                    reward.Image += "Pronz.png";
//                    break;
//                case 2:
//                    reward.Title = "ميدالية فضية";
//                    reward.Description = "مبروك لقد حصلت على الميدالية الفضية في مادة " + reward.Subject;
//                    reward.Image += "Silver.png";
//                    break;
//                case 3:
//                    reward.Title = "ميدالية ذهبية";
//                    reward.Description = "مبروك لقد حصلت على الميدالية الذهبية في مادة " + reward.Subject;
//                    reward.Image += "Gold.png";
//                    break;
//                case 4:
//                    reward.Title = "ميدالية بلاتينية";
//                    reward.Description = "مبروك لقد حصلت على الميدالية البلاتينية في مادة " + reward.Subject;
//                    reward.Image += "platin.png";
//                    break;
//                case 5:
//                    reward.Title = "الكأس";
//                    reward.Description = "مبروك لقد حصلت على الكأس في مادة " + reward.Subject;
//                    reward.Image += "Cub.png";
//                    break;
//                default:
//                    break;
//            }
//            LMSdb.LMSRewards.InsertOnSubmit(reward);
//            LMSdb.SubmitChanges();
//            int MedalNo = (from X in LMSdb.LMSRewards
//                            where X.MedalLevel == reward.MedalLevel && X.UserId == UserId
//                            select X).Count();
//            if (MedalNo == 3)
//            {
//                reward.myDate = System.DateTime.Now;
//                reward.Type = 2;
//                reward.MedalLevel = LevelAfterActivity;
//                // reward.Subject = Lesson.Unit.curriculum.shortname;
//                reward.UserId = UserId;
//                reward.IsNew = true; 
//                switch (LevelAfterActivity)
//                {
//                    case 1:
//                        reward.Title = "برنز";
//                        reward.Description = "مبروك لقد وصلت إلى المنصة البرنزية ";
//                        reward.Image = "Pronz.png";
//                        break;
//                    case 2:
//                        reward.Title = "فضة";
//                        reward.Description = "مبروك لقد وصلت إلى المنصة الفضية ";
//                        reward.Image = "Silver.png";
//                        break;
//                    case 3:
//                        reward.Title = "ذهب";
//                        reward.Description = "مبروك لقد وصلت إلى المنصة الذهبية ";
//                        reward.Image = "Gold.png";
//                        break;
//                    case 4:
//                        reward.Title = " بلاتين";
//                        reward.Description = "مبروك لقد وصلت إلى المنصة البلاتينية ";
//                        reward.Image = "platin.png";
//                        break;
//                    case 5:
//                        reward.Title = "البطولة";
//                        reward.Description = "مبروك لقد وصلت إلى منصة البطولة ";
//                        reward.Image = "Cub.png";
//                        break;
//                    default:
//                        break;
//                }
//                LMSdb.LMSRewards.InsertOnSubmit(reward);
//                LMSdb.SubmitChanges();
//            }
//        }
//        Error e = (from X in LMSdb.Errors
//                   where X.ErrorCode == 0
//                   select X).Single();
//        ReturnedResult = JsonConvert.SerializeObject(e);
//    }
//    else
//    {
//        ReturnedResult = JsonConvert.SerializeObject(IsAjaxRequestResult);
//    }
//    return ReturnedResult;
//}

