namespace SharedModule.Utilities
{
    public static class CodeMappers
    {
        public static string TPayCodeMapper(int code, string defaultMessage)
          => code switch
          {
              0 => "تم الإشتراك بنجاح",
              4 => "عذرًا ، ليس لديك رصيد كافٍ.",
              11 => "عذرًا ، رمز التحقيق غير صالح",
              14 => "عذرًا ، لا يمكن استخدام خط شركات",
              16 => "عذرًا ،  انتهت مدة رمز التحقيق",
              51 => "يوجد خطا في عملية الدفع ... نرجو مراجعة البيانات و إعادة المحاولة",
              _ => defaultMessage,
          };
    }
}
