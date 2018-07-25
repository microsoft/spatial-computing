using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DocumentDB imports
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using Microsoft.MR;

public class DocumentDBConnector : MonoBehaviour {

    #region Unity Inspector Variables
    [Tooltip("The Endpoint URL for Cosmos DB in DocumentDB.")]
    [SecretValue("DocumentDB.EndpointURL")]
    public string EndpointUrl;

    [Tooltip("The Primary Key for Cosmos DB in DocumentDB.")]
    [SecretValue("DocumentDB.PrimaryKey")]
    public string PrimaryKey;

    #endregion
    
    void Awake() {
        ServiceLocator.RegisterService<IDocumentClient, DocumentClient>((sp) => {
            sp.Add(typeof(IDocumentClient), new DocumentClient(new Uri(EndpointUrl), PrimaryKey));
        });
    }
}
