namespace SchoolERPAIChatbot.API.Services.Schemas
{
    public static class AIIntentSchema
    {
        public static BinaryData GetSchema()
        {
            return BinaryData.FromString(
                """
                {
                  "type": "object",
                  "properties": {
                    "intent": {
                      "type": "string",
                      "enum": [
                        "AttendanceEnquiry",
                        "FeeEnquiry",
                        "StudentEnquiry",
                        "AdmissionEnquiry",
                        "GeneralQuestion"
                      ]
                    },
                    "category": {
                      "type": "string",
                      "enum": [
                        "Attendance",
                        "FeeManagement",
                        "StudentManagement",
                        "Admission",
                        "General"
                      ]
                    },
                    "confidence": {
                      "type": "number"
                    },
                    "requiresDatabaseLookup": {
                      "type": "boolean"
                    },
                    "reason": {
                      "type": "string"
                    }
                  },
                  "required": [
                    "intent",
                    "category",
                    "confidence",
                    "requiresDatabaseLookup",
                    "reason"
                  ],
                  "additionalProperties": false
                }
                """
            );
        }
    }
}
