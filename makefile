build:
	dotnet publish ./oobe.csproj -c Release -o dist -r linux-x64 --self-contained true

dist: build
	dotnet publish ./oobe.csproj -c Release -o dist -r linux-x64 --self-contained true
	rm -rf app
	mkdir app
	cp -r ./src/* app/
	cp ./dist/oobe app/etc/pioneos/oobe

	chmod +x app/DEBIAN/postinst
	chmod +x app/DEBIAN/prerm

	dpkg-deb --build app
	mv app.deb ./dist/pioneos-oobe_1.0-9_amd64.deb
	rm -rf app
	rm -rf dist
	rm -rf bin
	rm -rf obj
clean:
	rm -rf app
	rm -rf dist
	rm -rf bin
	rm -rf obj
