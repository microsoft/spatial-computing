using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DocumentDBTester : MonoBehaviour {

    [Tooltip("UI Button which create a collection in Cosmos DB.")]
    public Button CreateCollectionButton;

    [Tooltip("UI Button which create a database in Cosmos DB.")]
    public Button CreateDatabaseButton;

    private DocumentClient Client
    {
        get
        {
            if (client == null)
            {
                client = (DocumentClient)ServiceLocator.GetService<IDocumentClient>();
            }
            return client;
        }
    }
    private DocumentClient client;

    void Start()
    {
        if (CreateCollectionButton != null)
        {
            CreateCollectionButton.onClick.AddListener(() =>
            {
                CreateCollection();
            });
        }

        if (CreateDatabaseButton != null)
        {
            CreateDatabaseButton.onClick.AddListener(() =>
            {
                CreateDatabase();
            });
        }
    }

    private void CreateDatabase()
    {
        Client.CreateDatabaseIfNotExistsAsync(new Database { Id = "UnitySDKTestDB" }).ContinueWith((response) => {
            Debug.Log("Done: " + response.Status);
        });
    }

    private void CreateCollection()
    {
        Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("UnitySDKTestDB"), new DocumentCollection { Id = "UnitySDKCollection" }).ContinueWith((response) => {
            Debug.Log("Done: " + response.Status);
        });
    }
}
