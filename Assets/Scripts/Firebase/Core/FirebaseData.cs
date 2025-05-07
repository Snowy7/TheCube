using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

namespace Firebase
{
    [FirestoreData]
    public class FirestoreDocument<T> where T : FirestoreDocument<T>
    {
        [FirestoreProperty] public string Id { get; set; }
        
        public FirestoreDocument() { }
        
        // virtual constructor from DocumentSnapshot
        public FirestoreDocument(DocumentSnapshot document)
        {
            
        }
        
        public virtual Task<bool> Load()
        {
            return Task.FromResult(false);
        }
        
        public virtual void UpdateWithSnapshot(DocumentSnapshot document)
        {
            Id = document.Id;
        }
        
        public static T FromJson(string json)
        {
            return JsonUtility.FromJson<T>(json) as T;
        }
        
        public static T FromDocument(DocumentSnapshot document)
        {
            return document.ConvertTo<T>();
        }
        
        public override string ToString()
        {
            return "";
        }
    }
}