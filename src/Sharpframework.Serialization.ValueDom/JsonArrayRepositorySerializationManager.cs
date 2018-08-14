
namespace Sharpframework.Serialization.ValueDom
{
    //using Serialization.Json;


    public abstract class JsonArrayRepositorySerializationManager<ArrayItemType, MyselfType>
        : ArrayRepositorySerializationManager<ArrayItemType, JsonValueDomAdapter, MyselfType>
    where MyselfType    : JsonArrayRepositorySerializationManager<ArrayItemType, MyselfType>
                        , new ()
    {
    } // End of Class JsonArrayRepositorySerializationManager<...>

    public sealed class JsonArrayRepositorySerializationManager<ArrayItemType>
        : JsonArrayRepositorySerializationManager<
            ArrayItemType,
            JsonArrayRepositorySerializationManager<ArrayItemType>>
    {
    } // End of Class JsonArrayRepositorySerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
