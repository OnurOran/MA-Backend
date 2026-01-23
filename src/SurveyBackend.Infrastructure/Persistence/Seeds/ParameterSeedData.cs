using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Parameters;

namespace SurveyBackend.Infrastructure.Persistence.Seeds;

public static class ParameterSeedData
{
    public static void SeedParameters(this ModelBuilder modelBuilder)
    {
        var parameters = new List<Parameter>
        {

            Parameter.Create(
                id: 1,
                code: null,
                groupName: "Parameters",
                displayName: "Parameters",
                name: "Parameters",
                description: null,
                parentId: 0,
                levelNo: 0,
                symbol: null,
                orderNo: 0
            ),

            Parameter.Create(
                id: 2,
                code: null,
                groupName: "SurveyAccessTypes",
                displayName: "Anket Erişim Türleri",
                name: "SurveyAccessTypes",
                description: "Anket erişim tipi parametreleri",
                parentId: 1,
                levelNo: 1,
                symbol: null,
                orderNo: 0
            ),

            Parameter.Create(
                id: 3,
                code: "INTERNAL",
                groupName: "SurveyAccessTypes",
                displayName: "Dahili",
                name: "Internal",
                description: "Sadece kurum içi erişim",
                parentId: 2,
                levelNo: 2,
                symbol: null,
                orderNo: 0
            ),

            Parameter.Create(
                id: 4,
                code: "PUBLIC",
                groupName: "SurveyAccessTypes",
                displayName: "Halka Açık",
                name: "Public",
                description: "Herkese açık erişim",
                parentId: 2,
                levelNo: 2,
                symbol: null,
                orderNo: 1
            ),

            Parameter.Create(
                id: 5,
                code: null,
                groupName: "QuestionTypes",
                displayName: "Soru Türleri",
                name: "QuestionTypes",
                description: "Anket soru tipi parametreleri",
                parentId: 1,
                levelNo: 1,
                symbol: null,
                orderNo: 1
            ),

            Parameter.Create(
                id: 6,
                code: "SINGLE_SELECT",
                groupName: "QuestionTypes",
                displayName: "Tekli Seçim",
                name: "SingleSelect",
                description: "Tek seçenek seçilebilen soru tipi",
                parentId: 5,
                levelNo: 2,
                symbol: null,
                orderNo: 0
            ),

            Parameter.Create(
                id: 7,
                code: "MULTI_SELECT",
                groupName: "QuestionTypes",
                displayName: "Çoklu Seçim",
                name: "MultiSelect",
                description: "Birden fazla seçenek seçilebilen soru tipi",
                parentId: 5,
                levelNo: 2,
                symbol: null,
                orderNo: 1
            ),

            Parameter.Create(
                id: 8,
                code: "OPEN_TEXT",
                groupName: "QuestionTypes",
                displayName: "Açık Metin",
                name: "OpenText",
                description: "Serbest metin girişi yapılabilen soru tipi",
                parentId: 5,
                levelNo: 2,
                symbol: null,
                orderNo: 2
            ),

            Parameter.Create(
                id: 9,
                code: "FILE_UPLOAD",
                groupName: "QuestionTypes",
                displayName: "Dosya Yükleme",
                name: "FileUpload",
                description: "Dosya yüklemesi yapılabilen soru tipi",
                parentId: 5,
                levelNo: 2,
                symbol: null,
                orderNo: 3
            ),

            Parameter.Create(
                id: 10,
                code: "CONDITIONAL",
                groupName: "QuestionTypes",
                displayName: "Koşullu",
                name: "Conditional",
                description: "Koşula bağlı olarak gösterilen soru tipi",
                parentId: 5,
                levelNo: 2,
                symbol: null,
                orderNo: 4
            )
        };

        modelBuilder.Entity<Parameter>().HasData(parameters);
    }
}
