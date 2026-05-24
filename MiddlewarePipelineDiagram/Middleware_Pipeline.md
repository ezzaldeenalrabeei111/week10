# مخطط خط أنابيب البرمجيات الوسيطة (Middleware Pipeline)
**إعداد: EZZALDEEN**

يوضح المخطط التالي كيفية تدفق طلب HTTP عبر البرمجيات الوسيطة المختلفة قبل الوصول إلى المتحكم (Controller) وكيفية عودة الاستجابة:

```mermaid
graph TD
    Request([HTTP Request]) --> M1[CorrelationIdMiddleware]
    M1 --> M2[RequestLoggingMiddleware]
    M2 --> M3[RequestTimingMiddleware]
    M3 --> Auth[Authentication/Authorization]
    Auth --> Routing[Routing/Endpoints]
    Routing --> Controller[Controller/Action]
    
    Controller --> Routing_Res[Routing]
    Routing_Res --> Auth_Res[Auth]
    Auth_Res --> M3_Res[RequestTimingMiddleware - STOP WATCH]
    M3_Res --> M2_Res[RequestLoggingMiddleware]
    M2_Res --> M1_Res[CorrelationIdMiddleware - ADD HEADER]
    M1_Res --> Response([HTTP Response])

    subgraph "تدفق الطلب (Request Pipeline)"
    M1
    M2
    M3
    end

    subgraph "تدفق الاستجابة (Response Pipeline)"
    M3_Res
    M2_Res
    M1_Res
    end
```

### أهمية ترتيب البرمجيات الوسيطة:
الترتيب الذي يتم به تسجيل البرمجيات الوسيطة في `Program.cs` أمر بالغ الأهمية:
- **Correlation ID:** يجب أن يكون في البداية لضمان وجود المعرف في كافة السجلات.
- **Logging:** يجب أن يكون مبكراً لالتقاط الطلب بالكامل.
- **Authentication:** يجب أن يسبق **Authorization**.
- **Authorization:** يجب أن يسبق **Endpoints**.

---
*تم إعداد هذا المخطط لتمثيل الترتيب المطبق في مشاريع الأسبوع العاشر.*
