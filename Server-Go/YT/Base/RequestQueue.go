package Base

type RequestQueue struct {
	requestQueue []*Request
}

func (queue *RequestQueue) Enqueue(request *Request) {
	if queue.requestQueue == nil {
		queue.requestQueue = make([]*Request, 0)
	}
	queue.requestQueue = append(queue.requestQueue, request)
}

func (queue *RequestQueue) Dequeue() *Request {
	if queue.Count() == 0 {
		return nil
	}
	request := queue.requestQueue[0]
	queue.requestQueue = queue.requestQueue[1:]
	return request
}

func (queue *RequestQueue) Count() int {
	return len(queue.requestQueue)
}
