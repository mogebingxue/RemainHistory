package YT

import (
	"ReaminHistory/YT/Base"
)

type RequestQueue struct {
	requestQueue []*Base.Request
}

func (queue *RequestQueue) Enqueue(request *Base.Request)  {
	if queue.requestQueue==nil{
		queue.requestQueue = make([]*Base.Request,0)
	}
	queue.requestQueue = append(queue.requestQueue, request)
}

func (queue *RequestQueue) Dequeue() *Base.Request {
	request :=queue.requestQueue[0]
	queue.requestQueue=queue.requestQueue[1:]
	return request
}

func (queue *RequestQueue) Count() int {
	return len(queue.requestQueue)
}
