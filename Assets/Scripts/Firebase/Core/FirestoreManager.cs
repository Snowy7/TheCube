using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using Firebase.Firestore;
using Snowy.Utils;

# if UNITY_EDITOR
using Unity.Multiplayer.Playmode;
# endif

namespace Firebase
{
    /// <summary>
    /// Manager for Firestore with Generic CRUD operations.
    /// </summary>
    public class FirestoreManager : MonoSingleton<FirestoreManager>
    {
        public override bool DestroyOnLoad => false;
        public bool IsReady { get; set; }

        FirebaseFirestore db;
        
        private void Start()
        {
            // in editor mode, do not initialize Firestore for clients
            # if UNITY_EDITOR
            if (CurrentPlayer.ReadOnlyTags().Contains("Client"))
            {
                return;
            }
            # endif
            
            FirestoreInit();
        }
        
        /// <summary>
        /// Initialize Firestore.
        /// </summary>
        void FirestoreInit()
        {
            IsReady = false;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    db = FirebaseFirestore.DefaultInstance;
                    IsReady = true;
                    Debug.Log("Firestore is ready to use!");
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        
        /// <summary>
        /// Add a document to a collection with auto-generated ID.
        /// </summary>
        /// <param name="collectionPath"></param>
        /// <param name="document"></param>
        /// <typeparam name="T"></typeparam>
        public async Task<DocumentReference> AddDocument<T>(string collectionPath, T document) where T : FirestoreDocument<T>
        {
            if (!string.IsNullOrEmpty(document.Id))
            {
                // use the custom ID
                return await AddDocument(collectionPath, document.Id, document);
            }
            
            DocumentReference docRef = null;
            
            await db.Collection(collectionPath).AddAsync(document).ContinueWith(async task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Document added successfully!");
                    // update the document ID
                    document.Id = task.Result.Id;
                    
                    // load the document
                    await document.Load();
                    
                    // get the document reference
                    docRef = db.Collection(collectionPath).Document(document.Id);
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error adding document: " + task.Exception);
                }
            });
            
            return docRef;
        }
        
        /// <summary>
        /// Add a document to a collection with a custom ID.
        /// </summary>
        /// <param name="collectionPath"></param>
        /// <param name="documentId"></param>
        /// <param name="document"></param>
        /// <typeparam name="T"></typeparam>
        public async Task<DocumentReference> AddDocument<T>(string collectionPath, string documentId, T document) where T : FirestoreDocument<T>
        {
            DocumentReference docRef = null;
            document.Id = documentId;
            
            await db.Collection(collectionPath).Document(documentId).SetAsync(document).ContinueWith(async task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Document added successfully!");
                    // update the document ID
                    document.Id = documentId;
                    
                    // load the document
                    await document.Load();
                    
                    // get the document reference
                    docRef = db.Collection(collectionPath).Document(documentId);
                    Debug.Log("Document reference: " + docRef.Path);
                    
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error adding document: " + task.Exception);
                }
            });
            
            return docRef;
        }
        
        /// <summary>
        /// Get a document from a collection by ID.
        /// </summary>
        /// <param name="collectionPath"></param>
        /// <param name="documentId"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public async Task<T> GetDocument<T>(string collectionPath, string documentId, System.Action<T> callback) where T : FirestoreDocument<T>
        {
            T doc = null;
            
            await db.Collection(collectionPath).Document(documentId).GetSnapshotAsync().ContinueWith(async task =>
            {
                if (task.IsCompleted)
                {
                    if (!task.Result.Exists)
                    {
                        Debug.LogWarning("Document does not exist!");
                        callback(null);
                        return;
                    }
                    doc = FirestoreDocument<T>.FromDocument(task.Result);
                    await doc.Load();
                    // update the document ID
                    doc.Id = documentId;
                    callback(doc);
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error getting document: " + task.Exception);
                }
            });
            
            return doc;
        }
        
        /// <summary>
        /// Get a document from a collection by reference.
        /// </summary>
        /// <param name="docRef"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetDocument<T>(DocumentReference docRef, System.Action<T> callback = null) where T : FirestoreDocument<T>
        {
            T doc = null;
            
            await docRef.GetSnapshotAsync().ContinueWith(async task =>
            {
                if (task.IsCompleted)
                {
                    if (!task.Result.Exists)
                    {
                        Debug.LogWarning("Document does not exist!");
                        if (callback != null) callback(null);
                        return;
                    }
                    doc = FirestoreDocument<T>.FromDocument(task.Result);
                    await doc.Load();
                    // update the document ID
                    doc.Id = docRef.Id;
                    callback?.Invoke(doc);
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error getting document: " + task.Exception);
                }
            });
            
            return doc;
        }
        
        /// <summary>
        /// Update a document in a collection by ID.
        /// </summary>
        /// <param name="collectionPath"></param>
        /// <param name="documentId"></param>
        /// <param name="document"></param>
        /// <typeparam name="T"></typeparam>
        public async Task UpdateDocument<T>(string collectionPath, string documentId, T document) where T : FirestoreDocument<T>
        {
            Debug.Log("Updating document for ID: " + documentId);
            await db.Collection(collectionPath).Document(documentId).SetAsync(document).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Document updated successfully!");
                    // update the document ID
                    document.Id = documentId;
                    
                    var docRef = db.Collection(collectionPath).Document(documentId);
                    Debug.Log("Document reference: " + docRef.Path);
                    // get snapshot
                    var snapshot = docRef.GetSnapshotAsync().Result;
                    Debug.Log("Document snapshot: " + snapshot);
                    document.UpdateWithSnapshot(snapshot);
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error updating document: " + task.Exception);
                }

                return Task.CompletedTask;
            });
            
            Debug.Log("Document updated for ID: " + documentId);
            
            // load the document
            await document.Load();
        }
        
        /// <summary>
        /// Delete a document from a collection by ID.
        /// </summary>
        /// <param name="collectionPath"></param>
        /// <param name="documentId"></param>
        public async Task DeleteDocument(string collectionPath, string documentId)
        {
            await db.Collection(collectionPath).Document(documentId).DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Document deleted successfully!");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error deleting document: " + task.Exception);
                }
            });
        }
    }
}