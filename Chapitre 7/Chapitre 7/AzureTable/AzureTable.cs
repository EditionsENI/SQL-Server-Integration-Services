using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;



namespace AzureTable
{
    [DtsPipelineComponent(DisplayName = "Source Azure Table",
    Description = "Ce composant permet le chargement des données depuis une table Azure",
    IconResource = "icone.ico",
    ComponentType = ComponentType.SourceAdapter)]
    public class AzureTable : PipelineComponent
    {


        public override void ProvideComponentProperties()
        {
            // Remise à zéro du composant
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            // Création de la sortie
            ComponentMetaData.OutputCollection.New();
            ComponentMetaData.OutputCollection[0].Name = "Output";

            // Proprités personnalisées 
            IDTSCustomProperty100 storageConnectionStringProperty = this.ComponentMetaData.CustomPropertyCollection.New();
            storageConnectionStringProperty.Name = "StorageConnectionString";
            storageConnectionStringProperty.Description = "Chaîne de connexion au stockage Azure";
            storageConnectionStringProperty.Value = "UseDevelopmentStorage=true";
            storageConnectionStringProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;

            IDTSCustomProperty100 tableNameProperty = this.ComponentMetaData.CustomPropertyCollection.New();
            tableNameProperty.Name = "TableName";
            tableNameProperty.Description = "Nom de la table";
            tableNameProperty.Value = string.Empty;
            tableNameProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;

        }

        //Methode de récupération d'une ligne de la Table Azure puis de création de colonnes SSIS Typées
        public void InitializeOutput()
        {
            //Suppression des colonnes existantes pour rechargement
            ComponentMetaData.OutputCollection[0].OutputColumnCollection.RemoveAll();
            //Récupération de la chaine de connexion au stockage blob Azure
            string storageConnectionString = (string)this.ComponentMetaData.CustomPropertyCollection["StorageConnectionString"].Value;
           
            string tableName = (string)this.ComponentMetaData.CustomPropertyCollection["TableName"].Value;
            
            //Connexion au stockage
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            //Récupération de la première ligne
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();
            query.TakeCount = 1;
            var row = table.ExecuteQuery(query).FirstOrDefault();

            if (row != null)
            {
                var properties = row.Properties;

                foreach (var prop in properties)
                {
                    //Création d'une colonne dans le composant source
                    IDTSOutputColumn100 newOutputCol = ComponentMetaData.OutputCollection[0].OutputColumnCollection.New();
                    //Le nom de la colonne est identique à la source
                    newOutputCol.Name = prop.Key;
                    //Le Type de données de la colonne est convertie en type SSIS puis attribué à la colonne du composant SSIS
                    newOutputCol.SetDataTypeProperties(this.GetSsisType(prop.Value).Key, this.GetSsisType(prop.Value).Value, 0, 0, 0);

                }
            }

        }
        //Méthode de conversion des types Azure Table Storage en Types SSIS
        private KeyValuePair<DataType, int> GetSsisType(EntityProperty propValue)
        {
            
            switch (propValue.PropertyType.ToString())
            {
                case "String": return new KeyValuePair<DataType, int>(DataType.DT_WSTR, propValue.PropertyAsObject.ToString().Length);
                case "int": return new KeyValuePair<DataType, int>(DataType.DT_I4, 0);
                case "long": return new KeyValuePair<DataType, int>(DataType.DT_I8, 0);
                case "bool": return new KeyValuePair<DataType, int>(DataType.DT_BOOL, 0); 
                case "DateTime": return new KeyValuePair<DataType, int>(DataType.DT_DATE,0);
                case "Guid": return new KeyValuePair<DataType, int>(DataType.DT_GUID, 0); 
                case "Double": return new KeyValuePair<DataType, int>(DataType.DT_R8, 0); 
                default:
                    return new KeyValuePair<DataType, int>(DataType.DT_NTEXT, 0); 
            }
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            //On récupère le buffer mémoire dans lequel seront stockées les lignes
            PipelineBuffer buffer = buffers[0];

            //Récupération de la chaine de connexion au stockage blob Azure
            string storageConnectionString = (string)this.ComponentMetaData.CustomPropertyCollection["StorageConnectionString"].Value;
            string tableName = (string)this.ComponentMetaData.CustomPropertyCollection["TableName"].Value;
            
            //Connexion au stockage
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);

            //Récupération des lignes
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();

            var azureTable = table.ExecuteQuery(query);

            foreach (DynamicTableEntity row in azureTable)
            {
                //Ajout d'une ligne au buffer
                buffer.AddRow();
                var properties = row.Properties;
                int i = 0;
                //Itération sur chaques colonnes
                foreach (var col in properties)
                {
                    if (col.Value != null)
                    {
                        buffer[i] = col.Value.PropertyAsObject;
                    }
                    else
                    {
                        buffer.SetNull(i);
                    }
                    i++;
                }
            }
            //Envoie du buffer
            buffer.SetEndOfRowset();
            
        }


        public override void ReinitializeMetaData()
        {
            InitializeOutput();
            base.ReinitializeMetaData();
        }

    
    }
}
