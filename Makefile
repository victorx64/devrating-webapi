#
# SPLS88 Makefile
#
ifeq ($(IMG_TAG),)
	API_IMG_TAG:=BE-$(shell git --git-dir=./.git rev-parse HEAD | cut -b 1-4)
else
	API_IMG_TAG:=$(IMG_TAG)
endif
ROOT_DIR:=$(shell dirname $(realpath $(lastword $(MAKEFILE_LIST))))

DOCKER_ADDR="registry.digitalocean.com/devrating"

build-and-push-drtng: build-and-push-drtng-api

build-and-push-drtng-api: build-drtng-api push-drtng-api

build-drtng-api:
	docker build -f Dockerfile -t "$(DOCKER_ADDR)/drtng-api:$(API_IMG_TAG)" .

push-drtng-api:
	docker push "$(DOCKER_ADDR)/drtng-api:$(API_IMG_TAG)"
