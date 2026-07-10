using UnityEngine;

[RequireComponent(typeof(Book))]
public class AutoFlip : MonoBehaviour
{
    public FlipMode Mode;
    public float PageFlipTime = 1;
    public float TimeBetweenPages = 1;
    public float DelayBeforeStarting = 0;
    public bool AutoStartFlip = true;
    public Book ControledBook;
    public int AnimationFramesCount = 40;

    bool isFlipping = false;

    // Variables para la animación con Update
    bool animating = false;
    float animationProgress = 0f;
    float startX, endX, xc, xl, h;
    FlipMode currentFlipMode;

    void Start()
    {
        if (!ControledBook)
            ControledBook = GetComponent<Book>();
        if (AutoStartFlip)
            Invoke("StartFlipping", DelayBeforeStarting);
        ControledBook.OnFlip.AddListener(new UnityEngine.Events.UnityAction(PageFlipped));
    }

    void PageFlipped()
    {
        isFlipping = false;
    }

    public void StartFlipping()
    {
        FlipRightPage();
    }

    void Update()
    {
        if (!animating) return;

        // Avanzamos la animación en función del tiempo real
        animationProgress += Time.deltaTime / PageFlipTime;
        animationProgress = Mathf.Clamp01(animationProgress);

        // Calculamos la X actual interpolando suavemente
        float currentX = Mathf.Lerp(startX, endX, animationProgress);
        float currentY = (-h / (xl * xl)) * (currentX - xc) * (currentX - xc);

        if (currentFlipMode == FlipMode.RightToLeft)
            ControledBook.UpdateBookRTLToPoint(new Vector3(currentX, currentY, 0));
        else
            ControledBook.UpdateBookLTRToPoint(new Vector3(currentX, currentY, 0));

        // Cuando termina la animación
        if (animationProgress >= 1f)
        {
            animating = false;
            isFlipping = false;
            ControledBook.ReleasePage();

            // Si es autoflip continuamos con la siguiente página
            if (AutoStartFlip)
                Invoke("FlipRightPage", TimeBetweenPages);
        }
    }

    public void FlipRightPage()
    {
        if (isFlipping) return;
        if (ControledBook.currentPage >= ControledBook.TotalPageCount) return;

        isFlipping = true;
        currentFlipMode = FlipMode.RightToLeft;

        xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;

        startX = xc + xl;
        endX = xc - xl;

        float startY = (-h / (xl * xl)) * (startX - xc) * (startX - xc);
        ControledBook.DragRightPageToPoint(new Vector3(startX, startY, 0));

        animationProgress = 0f;
        animating = true;
    }

    public void FlipLeftPage()
    {
        if (isFlipping) return;
        if (ControledBook.currentPage <= 0) return;

        isFlipping = true;
        currentFlipMode = FlipMode.LeftToRight;

        xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;

        startX = xc - xl;
        endX = xc + xl;

        float startY = (-h / (xl * xl)) * (startX - xc) * (startX - xc);
        ControledBook.DragLeftPageToPoint(new Vector3(startX, startY, 0));

        animationProgress = 0f;
        animating = true;
    }
}