using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// c2302845-023d-4c61-9b48-25d860a0c350_c6ad3001-dc81-45fd-a943-e7afa90c07f5

namespace test
{
    public sealed class C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelInput
    {
        public VideoFrame data { get; set; }
    }

    public sealed class C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> loss { get; set; }
        public C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelOutput()
        {
            this.classLabel = new List<string>();
            this.loss = new Dictionary<string, float>();
        }
    }

    public sealed class C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5Model
    {
        private LearningModelPreview learningModel;
        public static async Task<C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5Model> CreateC2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5Model(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5Model model = new C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5Model();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelOutput> EvaluateAsync(C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelInput input) {
            C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelOutput output = new C2302845_x002D_023d_x002D_4c61_x002D_9b48_x002D_25d860a0c350_c6ad3001_x002D_dc81_x002D_45fd_x002D_a943_x002D_e7afa90c07f5ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("data", input.data);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("loss", output.loss);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
