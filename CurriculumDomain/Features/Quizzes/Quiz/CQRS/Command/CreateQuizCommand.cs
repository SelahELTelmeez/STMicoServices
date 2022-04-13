using CurriculumDomain.Features.Quizzes.Quiz.DTO.Command;

namespace CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;
public record CreateQuizCommand(QuizRequest QuizRequest) : IRequest<CommitResult<int>>;

/*
 * 
 * 
 * 
 * private LMSQuizModel GetNewQuiz(int LOId, string UserId, string RequestType = "App")
    {
    int LessonId;
    int LessonType;
    int QuesionNo;
    int UnitId;
    int QuizDuration = 0;
    string SubjectId;
    clip LO = (from X in LMSdb.clips
    where X.id == LOId
    select X).Single();
    LessonId = LO.lessonid.Value;
    LessonType = LO.lesson.type.Value;
    QuesionNo = LO.pageno.Value;
    UnitId = LO.lesson.unitid.Value;
    SubjectId = LO.lesson.Unit.curricid;



var myvar = LMSdb.LMSCreateNewLessonQuiz(UserId, LessonId, LessonType, QuesionNo).ToList();
LMSQuizModel Quiz = new LMSQuizModel();



if (myvar.Count() > 0)
{
Quiz.QuizId = myvar[0].QuizId.Value;
Quiz.UnitId = UnitId;
Quiz.LessonId = LO.lessonid.Value;
Quiz.SubjectId = LO.lesson.Unit.curricid;




foreach (var item in myvar)
{
QuizDuration += item.Duration.Value;
LMSQuestion QuestionObj = new LMSQuestion();



QuestionObj.QuestionId = item.Id;



if (item.QuestionType != null)
QuestionObj.QuestionType = item.QuestionType.Value;
else
QuestionObj.QuestionType = 1;



if (item.QuestionType == 2)
{
item.Q = myMedia + SubjectId + "/mcq/" + item.Q;
item.A1 = myMedia + SubjectId + "/mcq/" + item.A1;
item.A2 = myMedia + SubjectId + "/mcq/" + item.A2;
if (!string.IsNullOrEmpty(item.A3))
item.A3 = myMedia + SubjectId + "/mcq/" + item.A3;
if (!string.IsNullOrEmpty(item.A4))
item.A4 = myMedia + SubjectId + "/mcq/" + item.A4;



}
QuestionObj.Hint = item.Hint;
QuestionObj.QuestionText = item.Q;
LMSChoice ch1 = new LMSChoice();
ch1.ChoiceId = 1;
ch1.ChoiceType = 1;
ch1.ChoiceContent = item.A1;



QuestionObj.ListChoice.Add(ch1);
QuestionObj.ListChoice.Add(new LMSChoice { ChoiceId = 2, ChoiceType = 1, ChoiceContent = item.A2 });



if (item.A3 != "")
{
QuestionObj.ListChoice.Add(new LMSChoice { ChoiceId = 3, ChoiceType = 1, ChoiceContent = item.A3 });



}
if (item.A4 != "")
{
QuestionObj.ListChoice.Add(new LMSChoice { ChoiceId = 4, ChoiceType = 1, ChoiceContent = item.A4 });



}
QuestionObj.CorrectChoice = Convert.ToInt32(item.CorrAnswer);



if (item.ExplanationLO != null)
{
var myclip = (from X in LMSdb.clips
where X.id == item.ExplanationLO
select X).ToList();
if (myclip.Count() > 0)
{



QuestionObj.ExplanatoryLO.LOPoints = myclip[0].LOPoints.Value;
string Type = myclip[0].cliptype.name;
string clipSubjectId = myclip[0].lesson.Unit.curricid;



QuestionObj.ExplanatoryLO.ID = myclip[0].id;
QuestionObj.ExplanatoryLO.Name = myclip[0].name;
QuestionObj.ExplanatoryLO.Type = myclip[0].type.Value;
QuestionObj.ExplanatoryLO.Orientation = myclip[0].Orientation.Value;



QuestionObj.ExplanatoryLO.Subject = SubjectId;
QuestionObj.ExplanatoryLO.UnitId = myclip[0].lesson.unitid.Value;
QuestionObj.ExplanatoryLO.KNLDBank = myclip[0].KNLDBank;
QuestionObj.ExplanatoryLO.W = myclip[0].w.Value;
QuestionObj.ExplanatoryLO.H = myclip[0].h.Value;




if (Type == "YouTube")
{
QuestionObj.ExplanatoryLO.MyFile = myclip[0].myfile;
QuestionObj.ExplanatoryLO.thum = myMedia + clipSubjectId + "/LOThumbnail/" + myclip[0].id.ToString() + ".png";
QuestionObj.ExplanatoryLO.LessonId = myclip[0].lessonid.Value;
QuestionObj.ExplanatoryLO.Subject = clipSubjectId;
QuestionObj.ExplanatoryLO.UnitId = myclip[0].lesson.unitid.Value;
}
else if (Type == "FlipHTML")
{



if (RequestType == "App")
QuestionObj.ExplanatoryLO.MyFile = myMedia + SubjectId+ "/" + Type + "/" + myclip[0].myfile;
else
QuestionObj.ExplanatoryLO.MyFile = myMedia + SubjectId + "/web" + Type + "/" + myclip[0].myfile;
QuestionObj.ExplanatoryLO.thum = myMedia + SubjectId + "/LOThumbnail/" + myclip[0].id.ToString() + ".png";
QuestionObj.ExplanatoryLO.LessonId = myclip[0].lessonid.Value;




}
else
{
QuestionObj.ExplanatoryLO.MyFile = myMedia + SubjectId + "/" + Type + "/" + myclip[0].myfile;
QuestionObj.ExplanatoryLO.thum = myMedia + SubjectId + "/LOThumbnail/" + myclip[0].id.ToString() + ".png";
QuestionObj.ExplanatoryLO.LessonId = myclip[0].lessonid.Value;
}




}



}



Quiz.ListQuestion.Add(QuestionObj);



}



Quiz.Duration = QuizDuration;




Quiz.Error = (from X in LMSdb.Errors
where X.ErrorCode == 0
select X).Single();



return Quiz;
}
else
{
Quiz.Error.ErrorCode = 10032;
Quiz.Error.ErrorMessage = "No Quizes in this Lesson";
return Quiz;
}
}
*/


/*
 * 
 * 
 * SP=>

ALTER PROCEDURE [dbo].[LMSCreateNewLessonQuiz]
(
@UserId nvarchar(255) = '',
@LessonId int,
@LessonType int ,
@QuestionNo int





)
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;



declare @UnitId int;
declare @subjectId nvarchar(50);
set @UnitId = (select unitId from lesson where id=@LessonId );
set @subjectId = (select curricid from unit where id = @UnitId);



-- ====================Insert Quiz in Quiz Table ======================
insert into LMSQuiz (SubjectId,UnitId,LessonId,QuizDate,QuizOwner) Values(@subjectID,@UnitId,@LessonId,getdate(),@UserId)
declare @QuizId int = @@IDENTITY




-- ===================== Generate Quiz's Questions ====================




if (@LessonType = 1)
begin

insert into LMSQuizQuestion SELECT top(@QuestionNo) @QuizId as QuizId
,mcq.[type] as QuestionType , dbo.mcq.corra as CorrAnswer, dbo.mcq.q AS Q, dbo.mcq.a1, dbo.mcq.a2 , dbo.mcq.a3, dbo.mcq.a4 , mcq.myclip as ExplanationLO , mcq.Hint , mcq.Duration
FROM dbo.mcq INNER JOIN
dbo.lesson ON dbo.mcq.lessonid = dbo.lesson.id
where lesson.id =@LessonId
order by newid()

end
else if(@LessonType = 2)
begin
insert into LMSQuizQuestion SELECT top(@QuestionNo) @QuizId as QuizId
,mcq.[type] as QuestionType , dbo.mcq.corra as CorrAnswer, dbo.mcq.q AS Q, dbo.mcq.a1, dbo.mcq.a2 , dbo.mcq.a3, dbo.mcq.a4 , mcq.myclip as ExplanationLO , mcq.Hint , mcq.Duration
FROM dbo.mcq INNER JOIN
dbo.lesson ON dbo.mcq.lessonid = dbo.lesson.id
where lesson.unitid= @UnitId
order by newid()
end
else if(@LessonType = 3)
begin



insert into LMSQuizQuestion SELECT top(@QuestionNo) @QuizId as QuizId
,mcq.[type] as QuestionType , dbo.mcq.corra as CorrAnswer, dbo.mcq.q AS Q, dbo.mcq.a1, dbo.mcq.a2 , dbo.mcq.a3, dbo.mcq.a4 , mcq.myclip as ExplanationLO , mcq.Hint , mcq.Duration
FROM dbo.mcq INNER JOIN
dbo.lesson ON dbo.mcq.lessonid = dbo.lesson.id INNER JOIN
dbo.unit ON dbo.lesson.unitid = dbo.unit.id
where unit.curricid=@subjectId
order by newid()
end
--==================== Select Quiz Questions =======================




SELECT * FROM LMSQuizQuestion
where LMSQuizQuestion.QuizId = @QuizId
order by newid()


END

*/