#if UNITY_WSA && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Media;
using Windows.Storage;

public sealed class Image_RecoModelInput
{
    public VideoFrame data { get; set; }
}

public sealed class Image_RecoModelOutput
{
    public IList<string> classLabel { get; set; }
    public IDictionary<string, float> loss { get; set; }
    public Image_RecoModelOutput()
    {
        this.classLabel = new List<string>();
        this.loss = new Dictionary<string, float>()
            {
                { "football", 0f },
                { "rubikscube", 0f }
            };
    }
}

public sealed class Image_RecoModel
{
    private LearningModelPreview learningModel;
    LearningModelBindingPreview binding;

    public static async Task<Image_RecoModel> CreateImage_RecoModel(StorageFile file)
    {
        LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
        Image_RecoModel model = new Image_RecoModel();
        model.learningModel = learningModel;
        model.binding = new LearningModelBindingPreview(learningModel);
        model.binding.Bind("classLabel", model.output.classLabel);
        model.binding.Bind("loss", model.output.loss);
        return model;
    }

    Image_RecoModelOutput output = new Image_RecoModelOutput();
    public async Task<Image_RecoModelOutput> EvaluateAsync(Image_RecoModelInput input)
    {
        if (input.data.Direct3DSurface == null)
        {
            return output;
        }

        var d = input.data.Direct3DSurface.Description;
        binding.Bind("data", input.data);
        LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
        return output;
    }
}
#endif
