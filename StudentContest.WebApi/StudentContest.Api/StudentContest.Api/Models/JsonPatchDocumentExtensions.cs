using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Models
{
    public static class JsonPatchDocumentExtensions
    {
        public static async Task ApplyToSafely(this JsonPatchDocument<Note?> patchDoc,
            Note? objectToApplyTo, INoteValidator noteValidator)
        {
            if (patchDoc == null) throw new ArgumentNullException(nameof(patchDoc));
            if (objectToApplyTo == null) throw new ArgumentNullException(nameof(objectToApplyTo));

            foreach (var op in patchDoc.Operations)
            {
                if (string.IsNullOrWhiteSpace(op.path)) continue;

                var segments = op.path.TrimStart('/').Split('/');
                var targetProperty = segments.First();
                switch (targetProperty)
                {
                    case "Status":
                        {
                            noteValidator.ValidateStatus(op.value);
                            break;
                        }
                    case "Id":
                    {
                        await noteValidator.ValidateId((int)op.value);
                        break;
                    }
                    case "Text":
                    {
                        noteValidator.ValidateText((string)op.value);
                        break;
                    }
                }
            }

            patchDoc.ApplyTo(objectToApplyTo, new ModelStateDictionary());
        }
    }
}
