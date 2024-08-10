build:
	dotnet publish ./oobe.csproj -c Release -o dist -r linux-x64 --self-contained true

dist: build
	rm -rf app
	mkdir app
	mkdir app/usr
	mkdir app/usr/bin
	mkdir app/DEBIAN
	cp -r src/. app/
	cp -r src/DEBIAN app/
	cp dist/oobe app/usr/share/pioneos/oobe
	chmod +x app/DEBIAN/postinst
	chmod +x app/DEBIAN/prerm

	dpkg-deb --build app
	mv app.deb ./pioneos-oobe_1.0-13_amd64.deb
	rm -rf app
	rm -rf dist
	rm -rf bin
	rm -rf obj
clean:
	rm -rf app
	rm -rf dist
	rm -rf bin
	rm -rf obj
